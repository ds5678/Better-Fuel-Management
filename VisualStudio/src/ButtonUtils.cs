using UnityEngine;

namespace BetterFuelManagement
{
	internal class ButtonUtils
	{
		internal static Vector3 GetBottomPosition(params Component[] components)
		{
			Vector3 result = new Vector3(0, 1000, 0);

			foreach (Component eachComponent in components)
			{
				if (eachComponent.gameObject.activeSelf && result.y > eachComponent.transform.localPosition.y)
				{
					result = eachComponent.transform.localPosition;
				}
			}

			return result;
		}

		internal static bool IsSelected(UIButton button)
		{
			Panel_Inventory_Examine_MenuItem menuItem = button.GetComponent<Panel_Inventory_Examine_MenuItem>();
			if (menuItem is null) return false;

			return menuItem.m_Selected;
		}

		internal static void SetButtonLocalizationKey(UIButton button, string key) => SetButtonLocalizationKey(button?.gameObject, key);

		internal static void SetButtonLocalizationKey(GameObject gameObject, string key)
		{
			if (gameObject is null) return;

			bool wasActive = gameObject.activeSelf;
			gameObject.SetActive(false);

			UILocalize localize = gameObject.GetComponentInChildren<UILocalize>();
			if (localize != null)
			{
				localize.key = key;
			}

			gameObject.SetActive(wasActive);
		}

		internal static void SetButtonSprite(UIButton button, string sprite)
		{
			if (button is null) return;

			button.normalSprite = sprite;
		}

		internal static void SetTexture(Component component, Texture2D texture)
		{
			if (component is null || texture is null) return;

			UITexture uiTexture = component.GetComponent<UITexture>();
			if (uiTexture is null) return;

			uiTexture.mainTexture = texture;
		}

		internal static void SetUnloadButtonLabel(Panel_Inventory_Examine panel, string localizationKey)
		{
			GameObject unloadPanel = ModComponent.Utils.ModUtils.GetChild(panel?.m_ExamineWidget?.gameObject, "UnloadRiflePanel");
			SetButtonLocalizationKey(unloadPanel, localizationKey);
		}
	}
}
