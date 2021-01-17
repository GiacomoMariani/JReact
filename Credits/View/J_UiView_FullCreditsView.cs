using JReact.UiView;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Credits
{
    public class J_UiView_FullCreditsView : MonoBehaviour
    {
#if UNITY_EDITOR
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_FullCredits _credits;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _heightAtEnd = 50;

        [BoxGroup("Category", true, true, 5), SerializeField, AssetsOnly, Required] private TextMeshProUGUI _categoryText;
        [BoxGroup("Category", true, true, 5), SerializeField] private float _categoryWidth = 900;
        [BoxGroup("Category", true, true, 5), SerializeField] private float _categoryHeight = 80;
        [BoxGroup("Category", true, true, 5), SerializeField] private float _heightAfterCategory = 40;

        [BoxGroup("Section", true, true, 10), SerializeField, AssetsOnly, Required] private TextMeshProUGUI _sectionText;
        [BoxGroup("Section", true, true, 10), SerializeField] private float _sectionWidth = 450;
        [BoxGroup("Section", true, true, 10), SerializeField] private float _sectionHeight = 80;
        [BoxGroup("Section", true, true, 10), SerializeField] private float _heightAfterSection = 40;

        [BoxGroup("Entry", true, true, 15), SerializeField, AssetsOnly, Required] private TextMeshProUGUI _entryText;
        [BoxGroup("Entry", true, true, 15), SerializeField] private float _entryWidth = 900;
        [BoxGroup("Entry", true, true, 15), SerializeField] private float _entryHeight = 80;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 50), ReadOnly, ShowInInspector] private float _xPosition;
        [FoldoutGroup("State", false, 50), ReadOnly, ShowInInspector] private float _yPosition;

        // --------------- COMMANDS --------------- //
        [BoxGroup("Commands", true, true, 100), Button]
        private void Populate()
        {
            SanityChecks();
            _xPosition = _yPosition = 0;
            for (int i = 0; i < _credits.Length; i++) { DrawCategory(_credits[i]); }

            GetComponent<J_UiView_RectMover>()?.SetFinalPosition(_yPosition - _entryHeight - _heightAtEnd);
        }

        [BoxGroup("Commands", true, true, 100), Button] private void ClearCredits() => transform.ClearTransform();

        private void DrawCategory(J_CreditCategory category)
        {
            // --------------- CREATE CATEGORY TITLE --------------- //
            //category reset x position
            _xPosition = 0;
            AddText(_categoryText, category.CategoryName, _categoryWidth, _categoryHeight, updateHeight: true);
            // --------------- FILL THE SECTIONS --------------- //
            for (int i = 0; i < category.Length; i++) DrawSection(category[i]);
            _yPosition -= _heightAfterCategory;
        }

        private void DrawSection(J_CreditSection section)
        {
            // --------------- CREATE SECTION TITLE --------------- //
            //section reset x position
            _xPosition = 0;
            AddText(_sectionText, section.SectionName, _sectionWidth, _sectionHeight, updateWidth: true);
            // --------------- FILL THE ENTRIES --------------- //
            for (int i = 0; i < section.Length; i++) { DrawEntry(section[i]); }

            _yPosition -= _heightAfterSection;
        }

        private void DrawEntry(string entry) { AddText(_entryText, entry, _entryWidth, _entryHeight, updateHeight: true); }

        // --------------- HELPERS --------------- //
        private void AddText(TextMeshProUGUI textPrefab, string text, float width, float height, bool updateWidth = false,
                             bool            updateHeight = false)
        {
            // --------------- CREATE THE ITEM --------------- //
            TextMeshProUGUI categoryItem = Instantiate(textPrefab, transform);
            categoryItem.rectTransform.anchoredPosition = new Vector2(_xPosition, _yPosition);
            categoryItem.rectTransform.sizeDelta        = new Vector2(width,      height);
            categoryItem.name                           = textPrefab.name + "_" + text;

            // --------------- SET THE TEXT --------------- //
            categoryItem.text = text;

            // --------------- UPDATE THE POSITIONS IF REQUESTED --------------- //
            if (updateWidth) _xPosition  += width;
            if (updateHeight) _yPosition -= height;
        }

        // --------------- CHECKS --------------- //
        private void SanityChecks()
        {
            CheckElement(_categoryText);
            CheckElement(_sectionText);
            CheckElement(_entryText);
        }

        private void CheckElement(TextMeshProUGUI element)
        {
            Assert.IsNotNull(element, $"{gameObject.name} missing a {nameof(element)}");
            RectTransform rect = element.rectTransform;
            Assert.IsTrue(rect.pivot == new Vector2(0, 1),
                          $"{gameObject.name} has a prefab {element.gameObject.name} with anchor {rect.pivot}. We need 0,1");
        }
#endif
    }
}
