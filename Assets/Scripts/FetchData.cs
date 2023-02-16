using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO;

public class FetchData : MonoBehaviour
{
    [Serializable]
    public struct Game
    {
        public string url;
        public int width;
        public int height;
    }

    private List<string> urllist = new List<string>();
    private List<string> path = new List<string>();
    private List<Sprite> sprites = new List<Sprite>();

  
    [SerializeField] GameObject uiRawImages;
    [SerializeField] GameObject newSprite;
    [SerializeField] GameObject checkerParent;

    private string apiURL = "https://quicklook.orientbell.com/Task/gettiles.php";
    private Vector3 imagePosition = Vector3.zero;
    private Game[] allGames;
    private bool firstImagePlaced = false;
    
    


    void Start()
    {

         StartCoroutine(FetchAPIData());
    }

    IEnumerator FetchAPIData()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiURL);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            // error ...

        }
        else
        {
            //success...
            allGames = JsonHelper.GetArray<Game>(request.downloadHandler.text);
            urllist.Add(allGames[0].url);
            urllist.Add(allGames[1].url);
            foreach (var item in urllist)
            {
                GameObject obj = Instantiate(uiRawImages, this.transform);
                obj.GetComponent<ImageDownloader>().setImageURL(item);
                obj.GetComponent<RawImage>().enabled = false;
              
            }

            yield return new WaitForSeconds(2);
            createSprite();

        }

        // Clean up any resources it is using.
        request.Dispose();
    }

    void createSprite()
    {
        
        for(int i=0;i<gameObject.transform.childCount;i++)
        {
            
            string s = gameObject.transform.GetChild(i).GetComponent<ImageDownloader>().imageName;
            Debug.Log(i);
            Debug.Log(Application.persistentDataPath + s);
            path.Add(Application.persistentDataPath + s);
            sprites.Add(Sprite.Create(LoadTextureFromPath(path[i]), new Rect(0, 0, 120, 60), new Vector2(0, 0)));
            Debug.Log(sprites[i]);
            //Debug.Log(path[i]);

        }
        checkerPattern();


        


    }


    void checkerPattern()
    {
        for(int j=0;j<6;j++)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject go = Instantiate(newSprite,checkerParent.transform);
                if (firstImagePlaced)
                {
                    go.GetComponent<SpriteRenderer>().sprite = sprites[0];

                }
                else
                {
                    go.GetComponent<SpriteRenderer>().sprite = sprites[1];
                }
                go.transform.position = imagePosition;
                firstImagePlaced = !firstImagePlaced;
                imagePosition.x += 120f / 100;
            }
            imagePosition.x = 0;
            imagePosition.y -= 60f / 100;
            firstImagePlaced = !firstImagePlaced;
        }

        

    }



    private Texture2D LoadTextureFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Texture path is null or empty");
            return null;
        }

        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(120,60, TextureFormat.RGB24, false);
        texture.LoadImage(bytes);
        Debug.Log("success");

        return texture;
    }

}
