using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Miyamasu.SlackIntegration {
    
    /**
        send log to slack.
    */
    public class _SendLog : CustomYieldInstruction {
        private readonly IEnumerator t;
        public _SendLog (string message, int type) {
            this.t = _WaitCor(message, type);
        }

        public override bool keepWaiting {
            get {
                return t.MoveNext();
            }
        }

        private class SlackMessage {
            public string text;
            public string channel;
            public SlackMessage (string message, string channelName, int type) {
                this.text = message;
                this.channel = channelName;
            }
        }

        private IEnumerator _WaitCor (string message, int type) {

            if (string.IsNullOrEmpty(Settings.staticSettings.slackToken)) {
                yield break;
            }
            if (string.IsNullOrEmpty(Settings.staticSettings.slackChannelName)) {
                yield break;
            }

            /*
                curl -X POST -H 'Authorization: Bearer xoxp-XXXX' -H 'Content-type: application/json' 
                --data '{"channel":"miyamasu","text":"I hope"}' https://slack.com/api/chat.postMessage
                */
            var uri = "https://slack.com/api/chat.postMessage";
            
            var data = JsonUtility.ToJson(new SlackMessage(message, Settings.staticSettings.slackChannelName, type));
            var http = new UnityWebRequest(uri, "POST");
            
            http.SetRequestHeader("Authorization", "Bearer " + Settings.staticSettings.slackToken);
            http.SetRequestHeader("Content-type", "application/json; charset=utf-8");
            var byteData = Encoding.UTF8.GetBytes(data);
            http.uploadHandler = new UploadHandlerRaw(byteData);

            // http.downloadHandler = new DownloadHandlerBuffer();

            var p = http.Send();

            while (!p.isDone) {
                yield return null;
            }

            // var error = http.error;
            // if (!string.IsNullOrEmpty(error)) {
            //     Debug.Log("error:" + error);
            // }

            // var code = http.responseCode;
            // Debug.Log("code:" + code);

            // var responseData = System.Text.Encoding.UTF8.GetString(http.downloadHandler.data);
            // Debug.Log("responseData:" + responseData);
        }
    }
    
    /**
        take screenshot then send to server.
    */
    public class _SendScreenshot : CustomYieldInstruction {
        private readonly IEnumerator t;

        public _SendScreenshot (string message) {
            t = _WaitCor(message);
        }

        public override bool keepWaiting {
            get {
                return t.MoveNext();
            }
        }

        private IEnumerator _WaitCor (string message) {
            if (string.IsNullOrEmpty(Settings.staticSettings.slackToken)) {
                yield break;
            }
            if (string.IsNullOrEmpty(Settings.staticSettings.slackChannelName)) {
                yield break;
            }

            var fileName = message.Replace(" ", "_") + "_screenshot_" + DateTime.Now.ToString().Replace(":", "_").Replace(" ", "_").Replace("/", "_");
            var basePath = Path.Combine(Application.persistentDataPath, fileName);
            
            // hide miyamasu UI.
            var testCanvas = GameObject.Find("MiyamasuCanvas");
            if (testCanvas != null) {
                var canvasAlpha = testCanvas.GetComponent<CanvasGroup>();
                canvasAlpha.alpha = 0;
            }

            // 古いUnityだと、end of frameで処理をしないとUIが撮影できない、みたいなことがあるかもしれない。
            // Unity5.6だとそのまま実行してUIが映る。

            if (Application.isMobilePlatform) {
                Application.CaptureScreenshot(fileName);// supersize = 0.
            } else {
                Application.CaptureScreenshot(basePath);// supersize = 0.
            }
            
            // wait 1 frame.
            yield return null;

            // show Miyamasu UI.
            if (testCanvas != null) {
                var canvasAlpha = testCanvas.GetComponent<CanvasGroup>();
                canvasAlpha.alpha = 1;
            }
            
            while (!File.Exists(basePath)) {
                yield return null;
            }

            // file found. start uploading.
            
            /*
                curl -F file=@scr.png 
                -F channels=#miyamasu 
                -F token=xoxp-XXXX 
                https://slack.com/api/files.upload
                */
            var uri = "https://slack.com/api/files.upload";
            

            var lockObj = new object();

            // ready multipart post request to slack.
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var multipartFormRequest = WebRequest.Create(uri) as HttpWebRequest;

            multipartFormRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            multipartFormRequest.Method = "POST";
            multipartFormRequest.KeepAlive = true;
            multipartFormRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // add form parameters.
            var formParameters = new NameValueCollection();
            formParameters.Add("channels", "#miyamasu");
            formParameters.Add("token", Settings.staticSettings.slackToken);
            
            
            using (var rs = multipartFormRequest.GetRequestStream()) {
                var formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in formParameters.Keys) {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, formParameters[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);
            
                var headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                var header = string.Format(headerTemplate, "file", fileName, "image/png");
                var headerbytes = Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);
                
                var length = 1024 * 100;// 100k
                var bytes = new byte[length];

                // send screenshot bytes with split by buffer size.
                using (var fStream = File.OpenRead(basePath)) {
                    var continuation = true;
                    
                    Action onDone = () => {
                        continuation = false;
                    };

                    // send recursive in async.
                    SendScreenshotBytesAsync(bytes, rs, fStream, lockObj, onDone);

                    while (continuation) {
                        yield return null;
                    }
                }
                
                // finished to send screenshot bytes.
                var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
            }
            
            // remove sended file.
            File.Delete(basePath);
            
            var response = multipartFormRequest.BeginGetResponse(
                ar => {
                    multipartFormRequest.EndGetResponse(ar);
                }, 
                lockObj
            );

            while (!response.IsCompleted) {
                yield return null;
            }

            // var wresp = multipartFormRequest.GetResponse();
            // Stream stream2 = wresp.GetResponseStream();
            // StreamReader reader2 = new StreamReader(stream2);
            // Debug.Log(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
        }


        private void SendScreenshotBytesAsync (byte[] bytes, Stream writeStream, FileStream readStream, object lockObj, Action onDone) {
            var rest = readStream.Length - readStream.Position;
            
            var readLength = bytes.Length;

            // use rest size if rest size is little than buffer size.
            if (rest < bytes.Length) {
                readLength = (int)rest;
            }

            // read file data async.
            readStream.BeginRead(
                bytes, 
                0, 
                readLength, 
                readAsyncResult => {
                    readStream.EndRead(readAsyncResult);

                    // write file data to server async.
                    writeStream.BeginWrite(
                        bytes, 
                        0, 
                        readLength,
                        writeAsyncResult => {
                            writeStream.EndWrite(writeAsyncResult);
                            
                            // no bytes remains.
                            if (readStream.Length - readStream.Position == 0) {
                                onDone();
                                return;
                            }

                            // continue sending rest bytes.
                            SendScreenshotBytesAsync(bytes, writeStream, readStream, lockObj, onDone);
                        }, 
                        lockObj
                    );
                }, 
                lockObj
            );
        }
    }
}
