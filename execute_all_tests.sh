#!/bin/bash

#create fresh build
#/Volumes/Samsung_T5/External\ Applications/unity/2019.3.13f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath ~/UnityCatanStarfarers/ -buildOSXUniversalPlayer ~/UnityCatanStarfarers/web_build.app

open ~/UnityCatanStarfarers/web_build.app
sleep 10

#/Volumes/Samsung_T5/External\ Applications/unity/2019.3.13f1/Unity.app/Contents/MacOS/Unity  -runTests -batchmode -projectPath ~/UnityCatanStarfarers/ -testResults ~/UnityCatanStarfarers/TestResults-EditMode.xml -testPlatform EditMode
/Volumes/Samsung_T5/External\ Applications/unity/2019.3.13f1/Unity.app/Contents/MacOS/Unity  -runTests -testFilter="Tests.MultiplayerTests.SendingResponseToCorrectPlayer" -batchmode -projectPath ~/UnityCatanStarfarers/ -testResults ~/UnityCatanStarfarers/TestResults-PlayMode.xml -testPlatform PlayMode

osascript -e 'quit app "web_build"'

# open ~/UnityCatanStarfarers/TestResults-EditMode.xml
open ~/UnityCatanStarfarers/TestResults-PlayMode.xml
