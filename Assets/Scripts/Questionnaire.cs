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
  public string customFileName = "Result_";

  private string[] questions = {
    "受試者",
    "這是第幾輪",
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
    "我完全被虛擬環境迷住了",
    "確認提交?"
  };

  private int questionnaireIndex = 2;

  private string[][] answers = new string[][] {
    new string[] {"001", "002", "003", "004", "005", "006", "007", "008", "009", "010", "011", "012", "013", "014"},
    new string[] {"第一輪", "第二輪", "第三輪"}
  };

  private List<string> selectedAnswers = new List<string>();

  void Start()
  {
    DisplayQuestion();
  }

  void DisplayQuestion()
  {
    // hide all answer buttons by default
      for (int i = 0; i < answerButtons.Length; i++)
      {
        answerButtons[i].gameObject.SetActive(false);
      }

    if (currentQuestionIndex == questions.Length - 1) {
      questionText.text = "(" + (currentQuestionIndex + 1) + "/" + questions.Length + ") " + questions[currentQuestionIndex];
      answerButtons[0].gameObject.SetActive(true);
      answerButtons[0].GetComponentInChildren<Text>().text = "確認";
      return;
    }

    if (currentQuestionIndex < questions.Length)
    {
      questionText.text = "(" + (currentQuestionIndex + 1) + "/" + questions.Length + ") " + questions[currentQuestionIndex];
      // show answer buttons based on the number of answers
      if (currentQuestionIndex < questionnaireIndex) {
        for (int i = 0; i < answers[currentQuestionIndex].Length; i++)
        {
          answerButtons[i].gameObject.SetActive(true);
          answerButtons[i].GetComponentInChildren<Text>().text = answers[currentQuestionIndex][i];
        }
      } else {
        for (int i = 0; i < 7; i++)
        {
          answerButtons[i].gameObject.SetActive(true);
          answerButtons[i].GetComponentInChildren<Text>().text = (i+1).ToString();
        }
      }
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
    string answer = index.ToString();
    if (currentQuestionIndex < questionnaireIndex)
    {
      answer = answers[currentQuestionIndex][index];
    }
    selectedAnswers.Add(answer);
    Debug.Log("Selected answer: " + index.ToString());
    currentQuestionIndex++;
    DisplayQuestion();
  }

  void SaveResults()
  {
    Debug.Log("Saving results...");
    questionText.text = "儲存中...";

    string timeStamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
    string fileName = $"{customFileName}{selectedAnswers[0]}_{timeStamp}.txt";
    string filePath = Path.Combine(Application.persistentDataPath, fileName);
    
    using (StreamWriter writer = new StreamWriter(filePath, false)) // 'false' overwrites file
    {
      for (int i = 0; i < selectedAnswers.Count-1; i++)
      {
        writer.WriteLine(selectedAnswers[i]);
      }
    }
    
    Debug.Log("Results saved at: " + filePath);
    questionText.text = "Results saved at: " + filePath;
  }

  public void Reset() {
    currentQuestionIndex = 0;
    selectedAnswers.Clear();
    DisplayQuestion();
  }

  public void Undo() {
    if (currentQuestionIndex > 0) {
      currentQuestionIndex--;
      selectedAnswers.RemoveAt(selectedAnswers.Count - 1);
      DisplayQuestion();
    }
  }
}