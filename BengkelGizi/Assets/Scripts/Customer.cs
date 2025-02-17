using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    [SerializeField] GameObject orderBox;
    [SerializeField] Transform orderTextParent;
    [SerializeField] Transform hearts;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource talk_SFX;

    [SerializeField] Sprite halfHeart;
    [SerializeField] Sprite emptyHeart;

    [Header("Customer Property")]
    [SerializeField] float orderTime = 3f;
    [SerializeField] float customerPatience = 25f;
    [SerializeField, Range(1, 5)] int minTotalOrder = 1;
    [SerializeField, Range(1, 5)] int maxTotalOrder = 3;
    [SerializeField, Range(1, 15)] int minNutritionValue = 1;
    [SerializeField, Range(1, 15)] int maxNutritionValue = 10;

    Nutrition nutrition;
    int totalOrder;
    int i_heart, j_heart;
    Coroutine waitForOrderRoutine = null;
    List<Nutrition> nutritionList = new List<Nutrition>();

    Dictionary<Nutrition, int> custOrder = new Dictionary<Nutrition, int>(5){
        {Nutrition.Karbohidrat, 0},
        {Nutrition.Protein, 0},
        {Nutrition.Serat, 0},
        {Nutrition.Mineral, 0},
        {Nutrition.Kalsium, 0}
    };

    private void Awake()
    {
        orderBox.SetActive(false);
        hearts.gameObject.SetActive(false);

        for (int i = 0; i < orderTextParent.childCount; i++)
        {
            orderTextParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(Order());
        UpdateOrder();
    }

    private void UpdateOrder()
    {
        totalOrder = Random.Range(minTotalOrder, maxTotalOrder + 1);

        while (totalOrder > 0)
        {
            nutrition = (Nutrition)Random.Range(0, 5);
            if (!nutritionList.Contains(nutrition))
            {
                nutritionList.Add(nutrition);

                custOrder[nutrition] = Random.Range(minNutritionValue, maxNutritionValue + 1);

                var text = orderTextParent.GetChild((int)nutrition);
                text.GetComponent<TMP_Text>().text += custOrder[nutrition].ToString();
                text.gameObject.SetActive(true);

                totalOrder--;
            }
        }
    }

    IEnumerator Order()
    {
        yield return new WaitForSeconds(orderTime);

        anim.SetBool("isTalking", true);
        talk_SFX.Play();
        yield return new WaitForSeconds(0.5f);

        orderBox.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        anim.SetBool("isTalking", false);
        talk_SFX.Stop();

        waitForOrderRoutine = StartCoroutine(WaitForOrder());
    }

    IEnumerator WaitForOrder()
    {
        yield return new WaitForSeconds(1f);
        hearts.gameObject.SetActive(true);

        for (i_heart = 0; i_heart < 5; i_heart++)
        {
            for (j_heart = 0; j_heart < 2; j_heart++)
            {
                yield return new WaitForSeconds(customerPatience / 10f);
                hearts.GetChild(i_heart).GetComponent<SpriteRenderer>().sprite = (j_heart == 0 ? halfHeart : emptyHeart);
            }
        }

        StartCoroutine(CustomerLeft());
    }

    IEnumerator CustomerLeft()
    {
        orderBox.SetActive(false);

        anim.SetBool("isTalking", true);
        talk_SFX.pitch = -2.5f;
        talk_SFX.Play();

        yield return new WaitForSeconds(2f);

        anim.SetBool("isTalking", false);
        talk_SFX.Stop();

        yield return new WaitForSeconds(1f);

        gone();
        Debug.Log("Customer Left");
    }

    public void CutHeart()
    {
        if (i_heart == 4)
        {
            hearts.GetChild(i_heart).GetComponent<SpriteRenderer>().sprite = emptyHeart;
            StopCoroutine(waitForOrderRoutine);
            StartCoroutine(CustomerLeft());
            return;
        }

        if (j_heart == 0)
        {
            hearts.GetChild(i_heart).GetComponent<SpriteRenderer>().sprite = emptyHeart;
        }
        else
        {
            hearts.GetChild(i_heart).GetComponent<SpriteRenderer>().sprite = emptyHeart;
            hearts.GetChild(i_heart + 1).GetComponent<SpriteRenderer>().sprite = halfHeart;
        }

        i_heart++;
    }

    private void gone()
    {
        Destroy(this.gameObject);
        GameManager.Instance.CustGone();
    }

    private void OnMouseDown()
    {
        Debug.Log("Customer Served");
        Destroy(this.gameObject);
        GameManager.Instance.CustServe();
    }
}
