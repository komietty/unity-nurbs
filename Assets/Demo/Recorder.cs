using UnityEngine;

namespace kmty.NURBS.Demo {
    public class Recorder : MonoBehaviour {

        public string folderName;
        public int framerate = 60;
        public int maxRecordSeconds = 180;
        public bool recode = false;
        int frameCount;
        bool recording;

        void StartRecording() {
            System.IO.Directory.CreateDirectory(folderName);
            Time.captureFramerate = framerate;
            frameCount = -1;
            recording = true;
        }

        void Update() {
            if (recode && !recording) StartRecording();
            if (!string.IsNullOrEmpty(folderName) && frameCount > 0 && frameCount < framerate * maxRecordSeconds && recode)
                ScreenCapture.CaptureScreenshot($"{folderName}/frame{frameCount.ToString("0000")}.png");
            frameCount++;
        }
    }
}