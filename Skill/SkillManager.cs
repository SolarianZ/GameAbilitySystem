using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Skill
{
    public class SkillManager : ISkillManager
    {
        private readonly ISkillOwner _skillOwner;

        private readonly ISkillActivator _skillActivator;

        private readonly object? _persistentContext;

        private readonly List<ISkill> _skills = new List<ISkill>();


        public SkillManager(ISkillOwner skillOwner, ISkillActivator skillActivator, object? persistentContext)
        {
            _skillOwner = skillOwner;
            _skillActivator = skillActivator;
            _persistentContext = persistentContext;

            _skillActivator.Initialize(_skillOwner, _persistentContext);
        }

        public void Tick(uint deltaTime)
        {
            // 优先tick技能实例
            for (int i = 0; i < _skills.Count; i++)
            {
                var skill = _skills[i];
                skill.Tick(deltaTime);
            }

            // 然后tick技能激活器
            _skillActivator.Tick(deltaTime);
        }

        public bool HasSkill(int skillId)
        {
            foreach (var skill in _skills)
            {
                if (skill.Id == skillId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool GiveSkill(ISkill skill)
        {
            if (skill == null)
            {
                throw new ArgumentNullException("Skill is null", nameof(skill));
            }

            if (_skills.Contains(skill))
            {
                return false;
            }

            _skills.Add(skill);
            _skillActivator.OnSkillEquipped(skill);

            return true;
        }

        public bool RemoveSkill(int skillId)
        {
            for (int i = 0; i < _skills.Count; i++)
            {
                var skill = _skills[i];
                if (skill.Id == skillId)
                {
                    _skills.RemoveAt(i);
                    skill.OnUnequip();
                    _skillActivator.OnSkillUnequipped(skill);

                    return true;
                }
            }

            return false;
        }

        public void RemoveAllSkills()
        {
            for (int i = 0; i < _skills.Count; i++)
            {
                var skill = _skills[i];
                skill.OnUnequip();
                _skillActivator.OnSkillUnequipped(skill);
            }
        }

        public bool BanSkill(int skillId)
        {
            foreach (var skill in _skills)
            {
                if (skill.Id != skillId)
                {
                    continue;
                }

                return skill.TryBan();
            }

            return false;
        }

        public bool UnbanSkill(int skillId)
        {
            foreach (var skill in _skills)
            {
                if (skill.Id != skillId)
                {
                    continue;
                }

                return skill.TryUnban();
            }

            return false;
        }
    }
}
