using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageDownloader : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;
    public string imageName = "";


    public void setImageURL(string url)
    {    
       StartCoroutine(GetImage(url));
    }

    IEnumerator GetImage(string url)
    {

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            // error ...

        }
        else
        {
            imageName = "/image" + gameObject.transform.GetSiblingIndex() + ".png";
            if (!System.IO.File.Exists(Application.persistentDataPath + imageName))
            {
                rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Texture2D texture = new Texture2D(rawImage.texture.width, rawImage.texture.height);
                texture = (Texture2D)rawImage.texture;
                byte[] bytes = texture.EncodeToPNG();
                
                System.IO.File.WriteAllBytes(Application.persistentDataPath + imageName, bytes);
                
            }
        }

        // Clean up any resources it is using.
        request.Dispose();
    }
}
