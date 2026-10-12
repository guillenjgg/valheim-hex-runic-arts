using HexRunicArts.Core.Constants;
using HexRunicArts.Models;
using HexRunicArts.Services.Lore;
using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HexRunicArts.Core.GUI
{
    internal static class RunicQuestTrackerGui
    {
        private static GameObject _panel;
        private static Text _objectiveText;
        private static Button _collapseButton;
        private static bool _collapsed;
        private const float ExpandedHeight = 140f;
        private const float CollapsedHeight = 48f;

        internal static void Init()
        {
            if (GUIManager.IsHeadless())
            {
                return;
            }

            GUIManager.OnCustomGUIAvailable += CreatePanel;
        }

        internal static void Destroy()
        {
            GUIManager.OnCustomGUIAvailable -= CreatePanel;

            if (_panel != null)
            {
                Object.Destroy(_panel);
            }

            _panel = null;
            _objectiveText = null;
            _collapseButton = null;
        }

        internal static void UpdateQuest(Player player)
        {
            if (player == null || _panel == null || _objectiveText == null)
            {
                return;
            }

            var progress = LoreProgressionService.GetSplinteringProgress(player);

            switch (progress)
            {
                case SplinteringProgressModel.NotStarted:
                    _objectiveText.text =
                        "Return to where your journey in Valheim begins.\n" +
                        "Find the forgotten runestone near the sacrificial stones.";
                    _panel.SetActive(true);
                    break;

                case SplinteringProgressModel.CollectingMaterials:
                    var materials = LoreObjectiveService.GetSplinteringMaterialProgress(player);
                    var heart = materials[ItemIdentifiers.EikthyrHeart];

                    _objectiveText.text =
                        "Retrieve Eikthyr's Heart.\n" +
                        $"Eikthyr's Heart: {heart.Current}/{heart.Required}\n" +
                        "Return to the runestone.";
                    _panel.SetActive(true);
                    break;

                case SplinteringProgressModel.Unlocked:
                    Hide();
                    break;
            }
        }

        internal static void Show()
        {
            if (_panel == null || Player.m_localPlayer == null)
            {
                return;
            }

            UpdateQuest(Player.m_localPlayer);
        }

        internal static void Hide()
        {
            if (_panel != null)
            {
                _panel.SetActive(false);
            }
        }

        private static void CreatePanel()
        {
            if (GUIManager.Instance == null || GUIManager.CustomGUIFront == null)
            {
                return;
            }

            if (_panel != null && _panel.transform.parent == GUIManager.CustomGUIFront.transform)
            {
                return;
            }

            if (_panel != null)
            {
                Object.Destroy(_panel);
            }

            _panel = null;
            _objectiveText = null;
            _collapseButton = null;

            _panel = GUIManager.Instance.CreateWoodpanel(
                parent: GUIManager.CustomGUIFront.transform,
                anchorMin: new Vector2(1f, 1f),
                anchorMax: new Vector2(1f, 1f),
                position: new Vector2(-150f, -380f),
                width: 280f,
                height: 140f,
                draggable: true);

            _panel.name = "HexRunicArts_QuestTracker";

            var title = GUIManager.Instance.CreateText(
                text: "The First Rune",
                parent: _panel.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, -20f),
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: 14,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: 250f,
                height: 75f,
                addContentSizeFitter: false);

            title.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

            var buttonObject = GUIManager.Instance.CreateButton(
                text: "-",
                parent: _panel.transform,
                anchorMin: new Vector2(1f, 1f),
                anchorMax: new Vector2(1f, 1f),
                position: new Vector2(-24f, -24f),
                width: 28f,
                height: 28f);

            _collapseButton = buttonObject.GetComponent<Button>();
            _collapseButton.onClick.AddListener(ToggleCollapsed);

            var objective = GUIManager.Instance.CreateText(
                text: "Return to where your journey began.\nFind the forgotten runestone near the sacrificial stones.",
                parent: _panel.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, -10f),
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 14,
                color: Color.white,
                outline: true,
                outlineColor: Color.black,
                width: 290f,
                height: 85f,
                addContentSizeFitter: false);

            objective.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

            _objectiveText = objective.GetComponent<Text>();

            _panel.SetActive(false);

            if (_collapsed)
            {
                _collapsed = false;
                ToggleCollapsed();
            }

            if (Player.m_localPlayer != null)
            {
                UpdateQuest(Player.m_localPlayer);
            }
        }

        private static void ToggleCollapsed()
        {
            if (_panel == null || _objectiveText == null || _collapseButton == null)
            {
                return;
            }

            _collapsed = !_collapsed;

            var rect = _panel.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                _collapsed ? CollapsedHeight : ExpandedHeight);

            _objectiveText.gameObject.SetActive(!_collapsed);
            _collapseButton.GetComponentInChildren<Text>().text = _collapsed ? "+" : "-";
        }
    }
}
