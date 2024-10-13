using System.Collections;
using System.Collections.Generic;
using AwakeComponents.SoftUI;
using AwakeComponents.SoftUI.Animations;
using AwakeComponents.SoftUI.Animations.DataTypes;
using AwakeComponents.Statistics;
using UnityEngine;
using UnityEngine.Events;

namespace AwakeComponents.SoftUI.SingleLevelMenu
{
    public class MenuNavigation : MonoBehaviour
    {
        public SoftUIAnimator menuAnimator;
        public SoftUIAnimator slideAnimator;

        public UnityEvent<int> onMenuItemSelected = new UnityEvent<int>();
        public UnityEvent<int> onMenuItemNavigated = new UnityEvent<int>();
        public UnityEvent onNavigateBackToMenu = new UnityEvent();

        public enum State
        {
            Menu,
            Slide,
            Animation
        }

        public State state = State.Menu;

        public void OnMenuItemSelected(int index)
        {
            if (state != State.Menu)
            {
                return;
            }

            state = State.Animation;

            StatisticsManager.Store("navigation.item_" + index);

            onMenuItemSelected.Invoke(index);

            menuAnimator.Animate(AnimationType.SlideUpOut);
            menuAnimator.Animate(AnimationType.AlphaOut, () =>
            {
                menuAnimator.gameObject.SetActive(false);
                slideAnimator.gameObject.SetActive(true);

                onMenuItemNavigated.Invoke(index);

                slideAnimator.WaitForChildAnimations(() => { state = State.Slide; });
            });
        }

        public void NavigateBackToMenu()
        {
            if (state != State.Slide)
            {
                return;
            }

            state = State.Animation;

            StatisticsManager.Store("navigation.home");

            onNavigateBackToMenu.Invoke();

            slideAnimator.WaitForChildAnimations(() =>
            {
                slideAnimator.gameObject.SetActive(false);

                menuAnimator.Animate(AnimationType.SlideDownIn);
                menuAnimator.Animate(AnimationType.AlphaIn, () => { state = State.Menu; });

                menuAnimator.gameObject.SetActive(true);
            });
        }
    }
}