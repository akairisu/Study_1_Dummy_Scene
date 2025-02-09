using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class Questionnaire : MonoBehaviour
{
  public GameObject questionnairePanel;
  public Text questionText;
  public GameObject answerButtonSet;
  public Button[] answerButtons;
  private int currentQuestionIndex = 0;

  private string[] questions = {
    "我認為我會願意經常使用此系統",
    "我發現此系統沒必要這麼複雜",
    "我認為此系統容易使用",
    "我認為我會需要技術人員的協助來使用此系統",
    "我認為此系統將不同的功能整合地很好",
    "我認為此系統有太多有太多不一致的地方 (too much inconsistency in this system)",
    "我認為大部分的人可以很快學會使用此系統",
    "我認為此系統使用起來非常笨拙 (cumbersome)",
    "我認為我可以非常有自信地使用此系統",
    "我認為我需要提前學習許多事項才可以順利地使用此系統",
    "我在虛擬環境中時，我對周圍的實體環境有非常清楚的感知 (周圍真實的聲音、溫度、其他人等)",
    "虛擬環境對我來說很真實",
    "我在虛擬環境中有一種參與感，而不是從外部操作的感覺",
    "虛擬環境與真實環境是一致的",
    "虛擬環境與實體環境無法區別",
    "在虛擬環境中，我有一種「在那裡」的感覺",
    "不知為何我覺得虛擬環境包圍了我",
    "我覺得我身處虛構的空間中",
    "我仍然關注周圍實體環境",
    "我認為虛擬環境比實體環境更為真實",
    "我覺得我只是在看靜態影像",
    "我完全被虛擬環境迷住了"
  };

  private List<int> selectedAnswers = new List<int>();

  void Start()
  {
    DisplayQuestion();
  }

  void DisplayQuestion()
  {
    if (currentQuestionIndex < questions.Length)
    {
      questionText.text = questions[currentQuestionIndex];
    }
    else
    {
      // questionnairePanel.SetActive(false);
      answerButtonSet.SetActive(false);
      SaveResults();
    }
  }

  public void OnAnswerSelected(int index)
  {
    selectedAnswers.Add(index);
    Debug.Log("Selected answer: " + index);
    currentQuestionIndex++;
    DisplayQuestion();
  }

  void SaveResults()
{
    Debug.Log("Saving results...");

    string timeStamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
    string fileName = $"Results_{timeStamp}.txt";
    string filePath = Path.Combine(Application.persistentDataPath, fileName);
    
    using (StreamWriter writer = new StreamWriter(filePath, false)) // 'false' overwrites file
    {
        foreach (int answer in selectedAnswers)
        {
            writer.WriteLine(answer);
        }
    }
    
    Debug.Log("Results saved at: " + filePath);
    questionText.text = "Results saved at: " + filePath;
}
}