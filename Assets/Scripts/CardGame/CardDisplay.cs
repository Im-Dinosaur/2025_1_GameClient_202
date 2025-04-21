using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData; // ī�� ������
    public int cardIndex;  //�� �п����� �ε���

    public MeshRenderer cardRenderer;  //ī�� ������(icon or �Ϸ���Ʈ)
    public TextMeshPro nameText;  //�̸� �ؽ�Ʈ
    public TextMeshPro costText;  //��� �ؽ�Ʈ
    public TextMeshPro attackText;  //���ݷ�/ȿ�� �ؽ�Ʈ
    public TextMeshPro descriptionText; //���� �ؽ�Ʈ

    public bool isDragging = false;
    private Vector3 originalPosition;  //�巡�� �� ���� ��ġ

    public LayerMask enemyLayer;  //�� ���̾�
    public LayerMask playerLayer;  //�÷��̾� ���̾�

    private CardManager cardManager;

    void Start()
    {
        //���̾� ����ũ ����
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");

        cardManager = FindObjectOfType<CardManager>();

        SetupCard(cardData);
    }
    //ī�� ������ ����
    public void SetupCard(CardData data)
    {
        cardData = data;

        //3D �ؽ�Ʈ ������Ʈ
        if (nameText != null) nameText.text = data.cardName;
        if (costText != null) costText.text = data.manaCost.ToString();
        if (attackText != null) attackText.text = data.effectAmount.ToString();
        if (descriptionText != null) descriptionText.text = data.description;


        //ī�� �ؽ��� ����
        if(cardRenderer != null && data.artwork != null)
        {
            Material cardMaterial = cardRenderer.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }
    }

    private void OnMouseDown()
    {
        //�巡�� ���� �� ���� ��ġ ����
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            //���콺 ��ġ�� ī�� �̵�
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
            Debug.Log($"������ �����մϴ�! (�ʿ� : {cardData.manaCost}, ���� : {playerStats?.currentMana ?? 0})");
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
                    Debug.Log($"{cardData.cardName} ī��� ������ {cardData.effectAmount} �������� �������ϴ�.");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("�� ī��� ������ ����� �� �����ϴ�.");
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
                    Debug.Log($"{cardData.cardName} ī��� �÷��̾��� ü���� {cardData.effectAmount} ȸ���߽��ϴ�!");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("�� ī��� �÷��̾�� ����� �� �����ϴ�.");
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
            Debug.Log($"������ {cardData.manaCost} ����߽��ϴ�. (���� ���� : {playerStats.currentMana})");
        }
    }

    void Update()
    {
        
    }
}
