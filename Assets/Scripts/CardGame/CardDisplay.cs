using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData; // 카드 데이터
    public int cardIndex;  //손 패에서의 인덱스

    public MeshRenderer cardRenderer;  //카드 렌더러(icon or 일러스트)
    public TextMeshPro nameText;  //이름 텍스트
    public TextMeshPro costText;  //비용 텍스트
    public TextMeshPro attackText;  //공격력/효과 텍스트
    public TextMeshPro descriptionText; //설명 텍스트

    public bool isDragging = false;
    private Vector3 originalPosition;  //드래그 전 원래 위치

    public LayerMask enemyLayer;  //적 레이어
    public LayerMask playerLayer;  //플레이어 레이어

    private CardManager cardManager;

    void Start()
    {
        //레이어 마스크 설정
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");

        cardManager = FindObjectOfType<CardManager>();

        SetupCard(cardData);
    }
    //카드 데이터 설정
    public void SetupCard(CardData data)
    {
        cardData = data;

        //3D 텍스트 업테이트
        if (nameText != null) nameText.text = data.cardName;
        if (costText != null) costText.text = data.manaCost.ToString();
        if (attackText != null) attackText.text = data.effectAmount.ToString();
        if (descriptionText != null) descriptionText.text = data.description;


        //카드 텍스쳐 설정
        if(cardRenderer != null && data.artwork != null)
        {
            Material cardMaterial = cardRenderer.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }
    }

    private void OnMouseDown()
    {
        //드래그 시작 시 원래 위치 저장
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            //마우스 위치로 카드 이동
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        ChracterStats playerStats = FindObjectOfType<ChracterStats>();
        if(playerStats == null || playerStats.currentMana < cardData.manaCost)
        {
            Debug.Log($"마나가 부족합니다! (필요 : {cardData.manaCost}, 현재 : {playerStats?.currentMana ?? 0})");
            transform.position = originalPosition;
            return;
        }

        isDragging = false;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool cardUsed = false;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            ChracterStats enemyStats = hit.collider.GetComponent<ChracterStats>();
            if (enemyStats != null)
            {
                if(cardData.cardType == CardData.CardType.Attack)
                {
                    enemyStats.TakeDamage(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} 카드로 적에게 {cardData.effectAmount} 데미지를 입혔습니다.");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("이 카드는 적에게 사용할 수 없습니다.");
                }
            }
        } 
        else if(Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
            //ChracterStats playerStats = hit.collider.GetComponent<ChracterStats>();

            if (playerStats != null)
            {
                if(cardData.cardType == CardData.CardType.Heal)
                {
                    playerStats.Heal(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} 카드로 플레이어의 체력을 {cardData.effectAmount} 회복했습니다!");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("이 카드는 플레이어에게 사용할 수 없습니다.");
                }
            }
        }
        else if(cardManager != null)
        {
            float distToDiscard = Vector3.Distance(transform.position, cardManager.discardPosition.position);
            if (distToDiscard < 2.0f)
            {
                cardManager.DiscardCard(cardIndex);
                return;
            }
        }

        if (!cardUsed)
        {
            transform.position = originalPosition;
            cardManager.ArrangeHand();
        }
        else
        {
            if(cardManager != null)
            {
                cardManager.DiscardCard(cardIndex);
            }

            playerStats.UseMana(cardData.manaCost);
            Debug.Log($"마나를 {cardData.manaCost} 사용했습니다. (남은 마나 : {playerStats.currentMana})");
        }
    }

    void Update()
    {
        
    }
}
