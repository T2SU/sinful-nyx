using Sevens.Entities;
using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sevens.Editor
{
    public class TestTool : EditorWindow
    {
        [MenuItem("Window/성준 테스트 툴")]
        static void Init()
        {
            var window = GetWindow<TestTool>("성준 테스트 툴");
            window.Show();
        }

        private int nyxDamage;
        private int nyxHeal;
        private int nyxSin;

        private bool updateRealtime;

        private void Update()
        {
            if (updateRealtime)
                Repaint();
        }

        void OnGUI()
        {
            GUILayout.Label("닉스 테스트", EditorStyles.boldLabel);
            //var obj = Selection.activeGameObject;
            var obj = GameObject.FindGameObjectWithTag("Player");
            Player nyx = null;
            if (obj?.GetComponent<Player>() is Player p)
            {
                nyx = p;
                using (var id = new EditorGUI.IndentLevelScope())
                {
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        nyxDamage = EditorGUILayout.IntField("데미지", nyxDamage);
                        if (GUILayout.Button("적용"))
                            nyx.OnDamagedBy(nyx, nyxDamage);
                    }
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        nyxHeal = EditorGUILayout.IntField("회복", nyxHeal);
                        if (GUILayout.Button("적용"))
                            nyx.Heal(nyxHeal);
                    }
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        nyxSin = EditorGUILayout.IntField("죄악", nyxSin);
                        //if (GUILayout.Button("적용"))
                        //    nyx.IncSin(nyxSin);
                    }
                }
            }
            else
                EditorGUILayout.HelpBox("씬에 닉스가 없습니다.", MessageType.Warning);
            GUILayout.Label("테스트 모드", EditorStyles.boldLabel);
            using (var id = new EditorGUI.IndentLevelScope())
            {
                updateRealtime = EditorGUILayout.Toggle("실시간 갱신", updateRealtime);
            //    using (var h = new EditorGUILayout.HorizontalScope())
            //    {
            //        DrawDebug.DebugGizmos = EditorGUILayout.Toggle("기즈모", DrawDebug.DebugGizmos);
            //    }
            }
            GUILayout.Label("씬의 몬스터", EditorStyles.boldLabel);
            using (var id = new EditorGUI.IndentLevelScope())
            {
                var mobs = new List<LivingEntity>();
                int mobLayer = LayerMask.NameToLayer("Mob");
                foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
                {
                    if (!go || !go.activeSelf)
                        continue;
                    if (go.layer != mobLayer)
                        continue;
                    var ent = go.GetComponent<LivingEntity>();
                    if (ent == null)
                        continue;
                    mobs.Add(ent);
                }

                GUILayout.Label($"{mobs.Count}개의 몹 개체");
                foreach (var mob in mobs)
                {
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"{mob.name} (Hp {mob.Hp:N0}/{mob.MaxHp:N0})");
                        if (GUILayout.Button("죽이기") && nyx != null && Application.isPlaying)
                            mob.OnDamagedBy(nyx, mob.Hp);
                    }
                }
            }
        }
    }
}