﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DialogManager : MonoBehaviour
{
  /// <summary>
  /// 对话文本文件，csv格式
  /// </summary>
  public TextAsset dialogDataFile;
  /// <summary>
  /// 左侧人物立绘
  /// </summary>
  public Image spriteBg;
  /// <summary>
  /// 角色名字文本
  /// </summary>
  public TextMeshProUGUI nameText;
  /// <summary>
  /// 对话文本，按行分割
  /// </summary>
  private string[] dialogRows;
  private int dialogIndex = 0;
  /// <summary>
  /// 对话内容文本
  /// </summary>
  public TextMeshProUGUI dialogText;
  public List<Sprite> sprites = new List<Sprite>();
  public Button nextButton;
  /// <summary>
  /// 选项按钮预制体
  /// </summary>
  public GameObject optionButton;
  /// <summary>
  /// 选项按钮父节点，用于排列
  /// </summary>
  public Transform buttonGroup;
  //bgm的AudioSource
  public AudioSource player;
  Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();
  private void Awake()
  {
    ReadText(dialogDataFile);
    imageDic["花园繁花似锦"] = sprites[0];
    imageDic["仙人抬手抚花"] = sprites[1];
    imageDic["仙人生气表情"] = sprites[2];
    imageDic["落花的花园，右下角有卷轴"] = sprites[3];
    imageDic["猫咪"] = sprites[4];
    imageDic["仙人微笑表情"] = sprites[5];
    imageDic["虾和蛙对视"] = sprites[6];
    imageDic["蛙和仙鹤对视"] = sprites[7];
    imageDic["仙鹤靠近猫尾巴和画卷"] = sprites[8];
    imageDic["仙鹤看到猫尾巴（远景）"] = sprites[9];
  }

  private void Start()
  {
    ShowDialogRow();

  }

  public void UpdateText(string _name, string _text)
  {
    // nameText.text = _name;
    if (_name == "右")
    {
      dialogText.canvasRenderer.SetAlpha(0.0f);
      dialogText.text = _text;
      dialogText.CrossFadeAlpha(1, 2.0f, false);
    }
    else
    {
      nameText.canvasRenderer.SetAlpha(0.0f);
      nameText.text = _text;
      nameText.CrossFadeAlpha(1, 2.0f, false);
    }
  }
  private IEnumerator MoveImgFromLeftToRight(Image _image)
  {
    float fromPos = 747.0f;
    float toPos = -747.0f;
    float speed = 500;
    while (fromPos > toPos)
    {
      fromPos -= speed * Time.fixedDeltaTime;
      _image.rectTransform.anchoredPosition3D = new Vector3(fromPos, 0, 0);
      yield return null;
    }
  }

  public IEnumerator UpdateImage(string _name)
  {
    // yield return new WaitForSeconds(0.8f);
    spriteBg.canvasRenderer.SetAlpha(0.0f);
    if (_name == "长图右")
    {
      StopCoroutine("MoveImgFromLeftToRight");
      spriteBg.rectTransform.sizeDelta = new Vector2(3415, 1080);
      spriteBg.rectTransform.anchoredPosition3D = new Vector3(-747.0f, 0, 0);
      spriteBg.sprite = imageDic["落花的花园，右下角有卷轴"];
      spriteBg.canvasRenderer.SetAlpha(1.0f);
      yield break;
    }
    else if (_name == "落花的花园，右下角有卷轴" || _name == "长图左" || _name == "长图动")
    {
      spriteBg.rectTransform.sizeDelta = new Vector2(3415, 1080);
      spriteBg.rectTransform.anchoredPosition3D = new Vector3(747.0f, 0, 0);
      spriteBg.sprite = imageDic["落花的花园，右下角有卷轴"];
      if (_name == "长图动")
      {
        StartCoroutine("MoveImgFromLeftToRight", spriteBg);
      }
    }
    else
    {
      if (spriteBg.sprite == imageDic[_name])
      {
        spriteBg.canvasRenderer.SetAlpha(1.0f);
        yield break;
      }
      spriteBg.rectTransform.sizeDelta = new Vector2(1920, 1080);
      spriteBg.rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
      spriteBg.sprite = imageDic[_name];
    }
    spriteBg.CrossFadeAlpha(1, 0.5f, false);
    yield return null;
  }

  public void ReadText(TextAsset _textAsset)
  {
    dialogRows = _textAsset.text.Split('\n');
  }
  public void ShowDialogRow()
  {
    for (int i = 0; i < dialogRows.Length; ++i)
    {
      string[] cells = dialogRows[i].Split(',');
      if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
      {
        nextButton.gameObject.SetActive(true);
        UpdateText(cells[3], cells[4]);
        StartCoroutine(UpdateImage(cells[2]));

        dialogIndex = int.Parse(cells[5]);
        break;
      }
      else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
      {
        nextButton.gameObject.SetActive(false);
        GenerateOption(i);
      }
      else if (cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
      {
        //AudioClip clip = Resources.Load<AudioClip>("music/bgmUI");
        //if(clip)
        //    player.clip = clip;
        //player.volume = 0.2f;
        //player.Play();
        Debug.Log("剧情结束");
        if (int.Parse(cells[6]) == 0)
        {
          gameObject.GetComponent<StartMenuManager>().OpenLevelSlection();
        }
        if (int.Parse(cells[6]) == 1)
        {
          SceneManager.LoadSceneAsync("stage_2");
        }
        if (int.Parse(cells[6]) == 2)
        {
          SceneManager.LoadSceneAsync("stage_3");
        }
        if (int.Parse(cells[6]) == 3)
        {
          SceneManager.LoadSceneAsync("DialogStage4");
        }
        if (int.Parse(cells[6]) == 4)
        {
          SceneManager.LoadSceneAsync("StartMenu");
        }
      }
    }
  }
  public void GenerateOption(int _index)
  {
    string[] cells = dialogRows[_index].Split(',');
    if (cells[0] == "&")
    {
      GameObject button = Instantiate<GameObject>(optionButton, buttonGroup);
      button.GetComponentInChildren<TextMeshProUGUI>().text = cells[4];
      button.GetComponent<Button>().onClick.AddListener(
        delegate
        {
          OnOptionClick(int.Parse(cells[5]));
        }
      );
      GenerateOption(_index + 1);
    }
  }
  public void OnClickNext()
  {
    ShowDialogRow();
    StartCoroutine("BanButtonClick", nextButton);
  }
  public void OnOptionClick(int _id)
  {
    dialogIndex = _id;
    ShowDialogRow();
    for (int i = 0; i < buttonGroup.childCount; ++i)
    {
      Destroy(buttonGroup.GetChild(i).gameObject);
    }
  }

  private IEnumerator BanButtonClick(Button _button)
  {
    _button.enabled = false;
    yield return new WaitForSeconds(1);
    _button.enabled = true;
  }
}
