using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{


    public void ChangeScene(String scene){
        SceneManager.LoadScene(1);
    }
}
