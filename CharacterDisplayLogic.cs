using RoR2;
using RoR2.Stats;
using UnityEngine;

namespace Crown_Mod;

public class SpinCrown : MonoBehaviour
{
    private Vector3 localPosBeforeOffset;

    private Transform parentTransform;

    private void Awake()
    {
        enabled = true;
    }

    private void Start()
    {
        parentTransform = transform.parent;
        localPosBeforeOffset = transform.localPosition;

        transform.parent = null;

        transform.rotation = Quaternion.identity;
        transform.localScale = CrownItemDisplayRule.GetCrownDisplayRule().localScale;
    }

    private void Update()
    {
        if (!parentTransform) {
            Destroy(this.gameObject);
        }

        transform.Rotate(Vector3.up * Time.deltaTime * 60, Space.Self);

        transform.position = parentTransform.position + localPosBeforeOffset + Mathf.Sin(Time.time * Mathf.PI * 0.5f) * Vector3.up * 0.05f;
    }
}

public class CharacterSpecific
{
    private static ItemDisplayRule CreateItemDisplayRuleDeepCopy(ItemDisplayRule toCopy)
    {
        ItemDisplayRule copy = new ItemDisplayRule();

        copy.childName = toCopy.childName;

        copy.localPos = toCopy.localPos;
        copy.localAngles = toCopy.localAngles;
        copy.localScale = toCopy.localScale;

        copy.limbMask = toCopy.limbMask;
        copy.ruleType = toCopy.ruleType;

        copy.followerPrefab = toCopy.followerPrefab;
        copy.followerPrefabAddress = toCopy.followerPrefabAddress;

        return copy;
    }

    public static ItemDisplayRule GetCrownDisplayRuleForCharacter(CharacterModel model)
    {
        ItemDisplayRule crownCharacterDispRule = CrownItemDisplayRule.GetCrownDisplayRule();

        switch (model.name)
        {
            case "mdlChef":
                crownCharacterDispRule = CreateItemDisplayRuleDeepCopy(crownCharacterDispRule);

                crownCharacterDispRule.localPos = Vector3.up * 1.1f;
                break;
            case "mdlFalseSon":
                crownCharacterDispRule = CreateItemDisplayRuleDeepCopy(crownCharacterDispRule);

                crownCharacterDispRule.localPos = Vector3.up * 0.9f;
                break;
            case "mdlCroco":
                crownCharacterDispRule = CreateItemDisplayRuleDeepCopy(crownCharacterDispRule);

                crownCharacterDispRule.localPos = Vector3.up * 0.9f;
                break;
            case "mdlEngi":
                crownCharacterDispRule = CreateItemDisplayRuleDeepCopy(crownCharacterDispRule);

                crownCharacterDispRule.localPos = new Vector3(0, 1.5f, 0.25f);
                crownCharacterDispRule.childName = "Base";
                break;
            case "mdlToolbot":
                crownCharacterDispRule = CreateItemDisplayRuleDeepCopy(crownCharacterDispRule);

                crownCharacterDispRule.localPos = Vector3.up * 0.9f;
                crownCharacterDispRule.childName = "Head";
                break;
            case "mdlCaptain":
                crownCharacterDispRule = CreateItemDisplayRuleDeepCopy(crownCharacterDispRule);

                crownCharacterDispRule.localPos = new Vector3(0, 1.5f, 0);
                crownCharacterDispRule.childName = "Base";
                break;
            case "mdlTreebot":
                crownCharacterDispRule = CreateItemDisplayRuleDeepCopy(crownCharacterDispRule);

                crownCharacterDispRule.localPos = new Vector3(0, 2.5f, 0);
                crownCharacterDispRule.childName = "Base";
                break;
            default:
                break;
        }

        return crownCharacterDispRule;
    }

}