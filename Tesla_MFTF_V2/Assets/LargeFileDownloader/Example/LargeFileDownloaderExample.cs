using UnityEngine;
using System.Collections;

// import large file downloader
using LargeFileDownloader;

public class LargeFileDownloaderExample : MonoBehaviour {

	const string FILE_URL_MP4 = "http://www.sample-videos.com/video/mp4/720/big_buck_bunny_720p_30mb.mp4";
	const string FILE_URL_FLV = "http://www.sample-videos.com/video/flv/720/big_buck_bunny_720p_30mb.flv";

	string fileToDownload;

	FileDownloader downloader;


	DownloadEvent evt = new DownloadEvent();

	// Use this for initialization
	void Start () { 
		

		// path to save video after downloading
		fileToDownload = FILE_URL_MP4;

		// create downloader instance
		downloader = new FileDownloader ();

		// add events listners
		FileDownloader.onComplete += OnDownloadComplete;
		FileDownloader.onProgress += OnProgress;


	}


	void OnDownloadComplete(DownloadEvent e)
	{
		evt = e;

		if (evt.error != null)
			Debug.Log (evt.error);
	}

	void OnProgress(DownloadEvent e)
	{
		evt = e;
	}


	void OnGUI()
	{
		fileToDownload = GUILayout.TextField (fileToDownload);
		if(GUILayout.Button("Download") && !downloader.IsInQueue(fileToDownload))
		{
			// start downloading
			string pathToSave = System.IO.Path.Combine (Application.persistentDataPath, System.IO.Path.GetFileName(fileToDownload));
			downloader.Download (fileToDownload, pathToSave);
			fileToDownload = FILE_URL_FLV;
		}

		if (GUILayout.Button ("Download in queue") && !downloader.IsInQueue(fileToDownload)) {
			string pathToSave = System.IO.Path.Combine (Application.persistentDataPath, System.IO.Path.GetFileName(fileToDownload));
			downloader.DownloadInQueue(fileToDownload,pathToSave);
		}

		if (GUILayout.Button ("Cancel")) {
			downloader.Cancel();
		}

		// status
		GUILayout.Label("Total Bytes : " +evt.totalBytes);
		GUILayout.Label("Downloaded Bytes : " +evt.downloadedBytes);
		GUILayout.Label("Downloading Progress (%): " +evt.progress);
		GUILayout.Label ("\nStatus : " + evt.status);
		GUILayout.Label ("\nError : " + ((!string.IsNullOrEmpty(evt.error)) ? evt.error : ""));

	}
}
