using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;

/*
    * File : ResourceManager.cs
    * Desc : Adressable resource를 관리하는 코드
    *
    &   [public]
    &   : LoadAtlas(string, string)     - Adressable resource 중 spriteAtlas load 기능
    &   : ReleaseAtlas()        - 로드된 spriteAtlas를 해제
    &   : LoadResource(string, UnityAction<Sprite>)     - Adressable resource 중 Sprite load 
    &   : ReleaseResource(string)           - 특정 resource 해제
    &   : ReleaseAllResource()           - 활용된 모든 resource 해제
    *
*/
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private Dictionary<string, AsyncOperationHandle<Sprite>> loadedSprites = new Dictionary<string, AsyncOperationHandle<Sprite>>();

    private AsyncOperationHandle<SpriteAtlas> atlasHandle;

    private SpriteAtlas spriteAtlas;
    
    private Sprite sprite;

    [SerializeField]
    private GameObject square;

    [SerializeField]
    private GameObject squareInst;

    [SerializeField]
    private GameObject squareA;

    [SerializeField]
    private GameObject squareInstA;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadAtlas(string atlasAddress, string whereCall)
    {
        spriteAtlas = null;

        if(!atlasAddress.Equals("Chap1_SA")){ 
            atlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasAddress);

            atlasHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    spriteAtlas = handle.Result;
                }
                else
                {
                    Debug.LogError("SpriteAtlas 로드 실패");
                }

                if(whereCall.Equals("HomeGrid") ){
                    SceneManager.LoadScene("TempDeskScene");
                }
                else if(whereCall.Equals("DeskScene") ){
                    SceneManager.LoadScene("TempDeskScene_Again");
                }else if(whereCall.Equals("LoadingSceneManagers")){
                    GameObject.Find("LoadingSceneManager").GetComponent<LoadingSceneManager>().StartLoadSceneOnRM();
                }
            };
        }
        else{
            if(whereCall.Equals("HomeGrid") ){
                SceneManager.LoadScene("TempDeskScene");
            }
            else if(whereCall.Equals("DeskScene") ){
                SceneManager.LoadScene("TempDeskScene_Again");
            }
        }
        
    }

    public void ReleaseAtlas()
    {
        if (atlasHandle.IsValid())
        {
            Addressables.Release(atlasHandle);
            atlasHandle = default; 
            Debug.Log("SpriteAtlas 해제됨");
        }
        else
        {
            Debug.Log("SpriteAtlas가 로드되지 않았습니다.");
        }
    }

    public void LoadResource(string address, UnityAction<Sprite> onLoaded)
    {
        sprite = null;
        if(spriteAtlas != null){
            sprite = spriteAtlas.GetSprite(address);
        }
        
        //해당 chapter의 Atlas에 이미지가 있다면
        if(sprite != null){
            onLoaded?.Invoke(sprite);
        }
        //해당 chapter의 atlas에 이미지가 없다면 addressable object에 직접 확인.
        else{
            // 이미 로드된 리소스가 있는지 체크
            if (loadedSprites.ContainsKey(address))
            {
                // 이미 로드된 리소스가 있으면 즉시 콜백 호출
                onLoaded?.Invoke(loadedSprites[address].Result);
            }
            else
            {
                // 리소스를 비동기적으로 로드
                AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
                handle.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        // 로드된 리소스를 딕셔너리에 저장
                        loadedSprites[address] = op;
                        // 콜백 호출
                        onLoaded?.Invoke(op.Result);
                    }
                    else
                    {
                        Debug.LogError("리소스 로드 실패: " + address);
                    }
                };
            }
        }

       
    }

    // 리소스 해제 메서드 (필요 시)
    public void ReleaseResource(string address)
    {
        if (loadedSprites.ContainsKey(address))
        {
            Addressables.Release(loadedSprites[address]);
            loadedSprites.Remove(address);
        }
    }

    public void ReleaseAllResource(){
        foreach (string key in loadedSprites.Keys)
        {
            Addressables.Release(loadedSprites[key]);   
        }

        loadedSprites.Clear();
        ReleaseAtlas();
    }
}
