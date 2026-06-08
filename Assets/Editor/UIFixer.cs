#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UIFixer
{
    [MenuItem("DungeonEscape/Fix UI (blur, font sizes, cursor)")]
    public static void FixUI()
    {
        // --- 1. DamageFlash en alta taşı (panellerin arkasında kalsın) ---
        var allImages = Resources.FindObjectsOfTypeAll<Image>();
        foreach (var img in allImages)
        {
            if (img.gameObject.name == "DamageFlash" && img.gameObject.scene.IsValid())
            {
                img.transform.SetAsFirstSibling();
                img.raycastTarget = false;
                Debug.Log("DamageFlash → Canvas'ın en altına taşındı");
            }
        }

        // --- 2. WinPanel ve LosePanel en üste taşı ---
        var allGOs = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allGOs)
        {
            if (!go.scene.IsValid()) continue;
            if (go.name == "WinPanel" || go.name == "LosePanel")
                go.transform.SetAsLastSibling();
        }

        // --- 3. Her panel için blur-overlay arka plan + yazı boyutları düzelt ---
        foreach (var go in allGOs)
        {
            if (!go.scene.IsValid()) continue;

            if (go.name == "WinPanel")
            {
                SetupPanel(go, new Color(0f, 0.08f, 0.22f, 0.92f));
                FixPanelTexts(go);
                FixPanelButtons(go, new Color(0.05f, 0.45f, 0.95f));
            }
            else if (go.name == "LosePanel")
            {
                SetupPanel(go, new Color(0.18f, 0f, 0f, 0.92f));
                FixPanelTexts(go);
                FixPanelButtons(go, new Color(0.85f, 0.12f, 0.12f));
            }
        }

        // --- 4. HUD yazıları biraz büyüt ---
        foreach (var tmp in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>())
        {
            if (!tmp.gameObject.scene.IsValid()) continue;
            string n = tmp.gameObject.name;
            if (n == "GemText" || n == "HealthText")
                tmp.fontSize = Mathf.Max(tmp.fontSize, 28f);
            if (n == "InfoText")
                tmp.fontSize = Mathf.Max(tmp.fontSize, 22f);
        }

        EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("UI Fixer tamamlandı — Ctrl+S ile kaydet");
    }

    static void SetupPanel(GameObject panel, Color bgColor)
    {
        // Mevcut arka plan Image'ı bul veya oluştur
        Image bg = panel.GetComponent<Image>();
        if (bg == null) bg = panel.AddComponent<Image>();
        bg.color = bgColor;
        bg.raycastTarget = true;

        // Panel'i tam ekran yap
        RectTransform rt = panel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // İçerik kutusunu bul (varsa) ve ortala
        Transform box = panel.transform.Find("Box");
        if (box == null) box = panel.transform.Find("Panel");
        if (box == null) box = panel.transform.Find("Content");
        if (box != null)
        {
            Image boxImg = box.GetComponent<Image>();
            if (boxImg == null) boxImg = box.gameObject.AddComponent<Image>();
            // Açık arka plan kutucuğu — içine blur hissi verir
            boxImg.color = new Color(0f, 0f, 0f, 0.55f);

            RectTransform brt = box.GetComponent<RectTransform>();
            if (brt != null)
            {
                brt.anchorMin = new Vector2(0.5f, 0.5f);
                brt.anchorMax = new Vector2(0.5f, 0.5f);
                brt.pivot     = new Vector2(0.5f, 0.5f);
                // Kutunun boyutunu ayarla (zaten varsa bozma)
                if (brt.sizeDelta.x < 300f)
                    brt.sizeDelta = new Vector2(500f, 380f);
            }
        }
    }

    static void FixPanelTexts(GameObject panel)
    {
        foreach (var tmp in panel.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            string n = tmp.gameObject.name.ToLower();

            if (n.Contains("title") || n.Contains("header") ||
                tmp.text.Contains("YOU") || tmp.text.Contains("ESCAPE"))
            {
                tmp.fontSize   = 72f;
                tmp.fontStyle  = FontStyles.Bold;
            }
            else if (n.Contains("score") || n.Contains("time"))
            {
                tmp.fontSize = 38f;
            }
            else
            {
                tmp.fontSize = Mathf.Max(tmp.fontSize, 32f);
            }

            tmp.enableAutoSizing = false;
        }
    }

    static void FixPanelButtons(GameObject panel, Color btnColor)
    {
        foreach (var btn in panel.GetComponentsInChildren<Button>(true))
        {
            // Buton rengini ayarla
            Image btnImg = btn.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.color = btnColor;

                var colors = btn.colors;
                colors.normalColor      = btnColor;
                colors.highlightedColor = btnColor * 1.3f;
                colors.pressedColor     = btnColor * 0.7f;
                colors.selectedColor    = btnColor;
                btn.colors = colors;
            }

            // Buton yazısını büyüt
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null)
            {
                tmp.fontSize  = 42f;
                tmp.fontStyle = FontStyles.Bold;
                tmp.color     = Color.white;
            }

            // Buton boyutunu düzelt
            RectTransform brt = btn.GetComponent<RectTransform>();
            if (brt != null && brt.sizeDelta.y < 60f)
                brt.sizeDelta = new Vector2(Mathf.Max(brt.sizeDelta.x, 280f), 70f);

            EditorUtility.SetDirty(btn.gameObject);
        }
    }
}
#endif
