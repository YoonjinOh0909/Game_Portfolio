using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : SetCharacterItem.cs
    * Desc : 스킨 세팅창에 의해 정보가 바뀔 때, 플레이어에 적용.
    *
    & 
    &   [public]
    &   : ChangeItemSkinImage()                - 플레이어에 설정된 스킨 적용.
    *
*/

public class SetCharacterItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer thisSprite;

    [SerializeField]
    private RewardCategory category;

    private GameObject itemPrefabs;

    protected bool canAdaptSkin= true;

    [SerializeField]
    private bool forFilter;

    private void Start() {
        ChangeItemSkinImage();
    }

    public void ChangeItemSkinImage(){
        if(canAdaptSkin && (forFilter == DataController.Instance.gameData.isColorFilterAssistant)){
            int curIndex = -1;
            switch(category){
                case RewardCategory.Glasses:
                    curIndex = DataController.Instance.gameData.currentGlassesIndex;
                    itemPrefabs = DataController.Instance.gameData.currentGlassesPrefabs;
                    break;
                case RewardCategory.Hat:
                    curIndex = DataController.Instance.gameData.currentHatIndex;
                    itemPrefabs = DataController.Instance.gameData.currentHatPrefabs;
                    break;
                case RewardCategory.Mask:
                    curIndex = DataController.Instance.gameData.currentMaskIndex;
                    itemPrefabs = DataController.Instance.gameData.currentMaskPrefabs;
                    break;
                default:
                    break;
            }


            if(this.gameObject.transform.childCount > 0 ){
                Transform[] ts = GetComponentsInChildren<Transform>();
                for(int i = 1; i < ts.Length; i++){
                    Destroy(ts[i].gameObject);
                }
            }
            
            if(itemPrefabs != null){
                Instantiate(itemPrefabs, this.gameObject.transform, false);
            }
        }
    }
}
