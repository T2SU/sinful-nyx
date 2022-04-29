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
        [MenuItem("Window/���� �׽�Ʈ ��")]
        static void Init()
        {
            var window = GetWindow<TestTool>("���� �׽�Ʈ ��");
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
            GUILayout.Label("�н� �׽�Ʈ", EditorStyles.boldLabel);
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
                        nyxDamage = EditorGUILayout.IntField("������", nyxDamage);
                        if (GUILayout.Button("����"))
                            nyx.OnDamagedBy(nyx, nyxDamage);
                    }
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        nyxHeal = EditorGUILayout.IntField("ȸ��", nyxHeal);
                        if (GUILayout.Button("����"))
                            nyx.Heal(nyxHeal);
                    }
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        nyxSin = EditorGUILayout.IntField("�˾�", nyxSin);
                        //if (GUILayout.Button("����"))
                        //    nyx.IncSin(nyxSin);
                    }
                }
            }
            else
                EditorGUILayout.HelpBox("���� �н��� �����ϴ�.", MessageType.Warning);
            GUILayout.Label("�׽�Ʈ ���", EditorStyles.boldLabel);
            using (var id = new EditorGUI.IndentLevelScope())
            {
                updateRealtime = EditorGUILayout.Toggle("�ǽð� ����", updateRealtime);
            //    using (var h = new EditorGUILayout.HorizontalScope())
            //    {
            //        DrawDebug.DebugGizmos = EditorGUILayout.Toggle("�����", DrawDebug.DebugGizmos);
            //    }
            }
            GUILayout.Label("���� ����", EditorStyles.boldLabel);
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

                GUILayout.Label($"{mobs.Count}���� �� ��ü");
                foreach (var mob in mobs)
                {
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"{mob.name} (Hp {mob.Hp:N0}/{mob.MaxHp:N0})");
                        if (GUILayout.Button("���̱�") && nyx != null && Application.isPlaying)
                            mob.OnDamagedBy(nyx, mob.Hp);
                    }
                }
            }
        }
    }
}