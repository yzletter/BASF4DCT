using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UIElements;
using System.Xml.Serialization;

public class NutBoneMovement : MonoBehaviour
{
    static int FileSize = 29;            //  文件夹中文件数量
    public string folderPath;
    public Vector3 targetPosition;      //  目标位置
    public Vector3 targetRotation;      //  目标旋转角度

    public string[] head = { "# rotation x: ", "# rotation y: ", "# rotation z: ", "# translation x: ", "# translation y: ", "# translation z: " };
    public float[,] nums = new float[FileSize, 6];    // 坐标值

    void Start()
    {
        ReadFile();         // 文件读取

        // 创建AnimationClip
        AnimationClip animationClip = new AnimationClip();
        animationClip.legacy = true;

        Transform myTranform = this.transform;

        // 动画帧
        Translation();      // 移动
        Rotation();         // 旋转

        // 创建Animation组件并添加AnimationClip
        Animation animation = gameObject.AddComponent<Animation>();
        animation.AddClip(animationClip, "MoveAndRotateAnimation");
        // 播放动画
        animation.Play("MoveAndRotateAnimation");


        //void ReadFile()
        //{
        //    TextAsset[] files = Resources.LoadAll<TextAsset>("舟骨矩阵变化");
        //    int i = 1;
        //    foreach (TextAsset file in files)
        //    {
        //        string[] lines = file.text.Split('\n');

        //        // 读取6 - 11行
        //        for (int col = 5; col < 11 && col < lines.Length; col++)
        //        {
        //            int index = lines[col].IndexOf(head[col - 5]);
        //            string content = lines[col].Substring(index + head[col - 5].Length);
        //            nums[i - 1, col - 5] = float.Parse(content);
        //        }

        //        i++;
        //    }
        //}

        // 函数封装
        void ReadFile()
        {
            //folderPath = Application.dataPath + "/舟骨矩阵变化";     // 指定文件夹路径
            Debug.Log(folderPath);
            for (int i = 1; i <= FileSize; i++)
            {
                // 读取文件
                string filePath = folderPath + "/0-" + i.ToString("G") + ".tfm";
                string[] lines = File.ReadAllLines(filePath);

                // 读取6 - 11行
                for (int col = 5; col < 11 && col < lines.Length; col++)
                {
                    int index = lines[col].IndexOf(head[col - 5]);
                    string content = lines[col].Substring(index + head[col - 5].Length);
                    nums[i - 1, col - 5] = float.Parse(content);
                }
            }
        }

        void Translation()
        {
            //  创建平移关键帧
            Keyframe[] XmoveKeys = new Keyframe[FileSize + 1];
            Keyframe[] YmoveKeys = new Keyframe[FileSize + 1];
            Keyframe[] ZmoveKeys = new Keyframe[FileSize + 1];

            //  初始位置
            XmoveKeys[0] = new Keyframe(0, myTranform.position.x);
            YmoveKeys[0] = new Keyframe(0, myTranform.position.y);
            ZmoveKeys[0] = new Keyframe(0, myTranform.position.z);



            // 目标位置
            for (int i = 1; i <= FileSize; i++)
            {
                targetPosition = new Vector3(nums[i - 1, 3], nums[i - 1, 4], nums[i - 1, 5]);
                XmoveKeys[i] = new Keyframe(i, targetPosition.x);
                YmoveKeys[i] = new Keyframe(i, targetPosition.y);
                ZmoveKeys[i] = new Keyframe(i, targetPosition.z);

            }

            AddTranslation();   // 将移动关键帧添加到AnimationClip中

            void AddTranslation()
            {
                AnimationCurve XmoveCurve = new AnimationCurve(XmoveKeys);
                AnimationCurve YmoveCurve = new AnimationCurve(YmoveKeys);
                AnimationCurve ZmoveCurve = new AnimationCurve(ZmoveKeys);
                animationClip.SetCurve("", typeof(Transform), "localPosition.x", XmoveCurve);
                animationClip.SetCurve("", typeof(Transform), "localPosition.y", YmoveCurve);
                animationClip.SetCurve("", typeof(Transform), "localPosition.z", ZmoveCurve);
            }
        }
        void Rotation()
        {
            // 创建旋转关键帧
            Keyframe[] XrotateKeys = new Keyframe[FileSize + 1];
            Keyframe[] ZrotateKeys = new Keyframe[FileSize + 1];
            Keyframe[] YrotateKeys = new Keyframe[FileSize + 1];

            // 初始位置
            XrotateKeys[0] = new Keyframe(0, myTranform.eulerAngles.x);
            YrotateKeys[0] = new Keyframe(0, myTranform.eulerAngles.y);
            ZrotateKeys[0] = new Keyframe(0, myTranform.eulerAngles.z);

            XrotateKeys[0].value = -90;

            for (int i = 1; i <= FileSize; i++)
            {
                targetRotation = new Vector3(nums[i - 1, 0], nums[i - 1, 1], nums[i - 1, 2]);
                XrotateKeys[i] = new Keyframe(i, targetRotation.x);
                YrotateKeys[i] = new Keyframe(i, targetRotation.y);
                ZrotateKeys[i] = new Keyframe(i, targetRotation.z);
            }

            AddRotation();  // 将旋转关键帧添加到AnimationClip中
            void AddRotation()
            {
                AnimationCurve YrotateCurve = new AnimationCurve(YrotateKeys);
                AnimationCurve XrotateCurve = new AnimationCurve(XrotateKeys);
                AnimationCurve ZrotateCurve = new AnimationCurve(ZrotateKeys);
                animationClip.SetCurve("", typeof(Transform), "localEulerAngles.x", XrotateCurve);
                animationClip.SetCurve("", typeof(Transform), "localEulerAngles.y", YrotateCurve);
                animationClip.SetCurve("", typeof(Transform), "localEulerAngles.z", ZrotateCurve);
            }
        }

    }
}
