# update defines.
echo -define:CLOUDBUILD > ./Assets/gmcs.rsp

# update date.
DATE=`date +%Y-%m-%d:%H:%M:%S`
echo //${DATE} > ./Assets/MiyamasuTestRunner/Editor/Timestamp.cs

/Applications/Unity5.4.2p4/Unity.app/Contents/MacOS/Unity -batchmode -quit -projectPath $(pwd) -executeMethod Miyamasu.CloudBuildTestEntryPoint.Test

rm ./Assets/gmcs.rsp
rm ./Assets/MiyamasuTestRunner/Editor/Timestamp.cs