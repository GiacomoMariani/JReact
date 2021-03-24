using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions
{
    /// <summary>
    /// checks multiple conditions
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Conditions/Multi", fileName = "MULTI_Condition")]
    public class J_MultiCondition : J_ReactiveCondition
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private OperatorType _operator = OperatorType.And;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveBool[] _trueConditions;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveBool[] _falseConditions;

        // --------------- INITIALIZATION METHODS --------------- //
        protected override void StartCheckingCondition()
        {
            ConditionsCheck(false);
            _trueConditions.SubscribeToAll(ConditionsCheck);
            _falseConditions.SubscribeToAll(ConditionsCheck);
        }

        protected override void StopCheckingCondition()
        {
            _trueConditions.UnSubscribeToAll(ConditionsCheck);
            _falseConditions.UnSubscribeToAll(ConditionsCheck);
        }

        // --------------- IMPLEMENTATIONM --------------- //
        protected override void UpdateCondition()
        {
            base.UpdateCondition();
            ConditionsCheck(false);
        }

        private void ConditionsCheck(bool item)
        {
            if (_operator == OperatorType.And) { AndOperator(); }
            else { OrOperator(); }
        }

        // --------------- AND OPERATOR --------------- //
        //checks AND conditions
        private void AndOperator()
        {
            Current = RequiresAllCondition(_trueConditions,  true) &&
                      RequiresAllCondition(_falseConditions, false);
        }

        private bool RequiresAllCondition(J_ReactiveBool[] collectionToCheck, bool expectedValue)
        {
            //check all true
            for (int i = 0; i < collectionToCheck.Length; i++)
            {
                Assert.IsNotNull(collectionToCheck[i], $"{name} null condition at: {i}");
                if (collectionToCheck[i].Current != expectedValue) { return false; }
            }

            return true;
        }

        // --------------- OR OPERATOR --------------- //
        private void OrOperator()
        {
            Current = RequiresOneCondition(_trueConditions,  true) ||
                      RequiresOneCondition(_falseConditions, false);
        }

        //check all the values
        private bool RequiresOneCondition(J_ReactiveBool[] collectionToCheck, bool expectedValue)
        {
            //check all true
            for (int i = 0; i < collectionToCheck.Length; i++)
            {
                Assert.IsNotNull(collectionToCheck[i], $"{name} null condition at: {i}");
                if (collectionToCheck[i].Current == expectedValue) { return true; }
            }

            return false;
        }
    }
}
