using System;
using static OutcastBot.Enumerations.Attributes;

namespace OutcastBot.Enumerations
{
    [Flags]
    public enum Mastery
    {
        [MasteryInfo("https://image.ibb.co/hxy4ya/00_Soldier.png", 0xF5D166)]
        Soldier = 1 << 0,

        [MasteryInfo("https://image.ibb.co/i8xKWv/01_Demolitionist.png", 0xE68033)]
        Demolitionist = 1 << 1,

        [MasteryInfo("https://image.ibb.co/crdRBv/02_Occultist.png", 0x4DAB26)]
        Occultist = 1 << 2,

        [MasteryInfo("https://image.ibb.co/jBEcJa/03_Nightblade.png", 0x4766FF)]
        Nightblade = 1 << 3,

        [MasteryInfo("https://image.ibb.co/bVoRBv/04_Arcanist.png", 0x00E8A8)]
        Arcanist = 1 << 4,

        [MasteryInfo("https://image.ibb.co/hKezWv/05_Shaman.png", 0x3399CC)]
        Shaman = 1 << 5,

        [MasteryInfo("https://image.ibb.co/bHXKWv/06_Inquisitor.png", 0x7370FF)]
        Inquisitor = 1 << 6,

        [MasteryInfo("https://image.ibb.co/ePG2jF/07_Necromancer.png", 0x8CD9CC)]
        Necromancer = 1 << 7,

        [MasteryInfo("https://image.ibb.co/gMnmBv/08_Commando.png", 0xEEA84C)]
        Commando = Soldier | Demolitionist,

        [MasteryInfo("https://image.ibb.co/dkV6Bv/09_Witchblade.png", 0xA1BE46)]
        Witchblade = Soldier | Occultist,

        [MasteryInfo("https://image.ibb.co/nnA6Bv/10_Blademaster.png", 0x9E9CB2)]
        Blademaster = Soldier | Nightblade,

        [MasteryInfo("https://image.ibb.co/iSPzWv/11_Battlemage.png", 0x7ADC87)]
        Battlemage = Soldier | Arcanist,

        [MasteryInfo("https://image.ibb.co/b2B2jF/12_Warder.png", 0x94B599)]
        Warder = Soldier | Shaman,

        [MasteryInfo("https://image.ibb.co/fAs8PF/13_Tactician.png", 0xB4A0B2)]
        Tactician = Soldier | Inquisitor,

        [MasteryInfo("https://image.ibb.co/hVJ4ya/14_Death_Knight.png", 0xC0D599)]
        DeathKnight = Soldier | Necromancer,

        [MasteryInfo("https://image.ibb.co/crZzWv/15_Pyromancer.png", 0x9A962C)]
        Pyromancer = Demolitionist | Occultist,

        [MasteryInfo("https://image.ibb.co/gb6Drv/16_Saboteur.png", 0x967399)]
        Saboteur = Demolitionist | Nightblade,

        [MasteryInfo("https://image.ibb.co/nqdF4F/17_Elementalist.png", 0x8C8C80)]
        Elementalist = Demolitionist | Shaman,

        [MasteryInfo("https://image.ibb.co/hEATPF/18_Sorcerer.png", 0x73B46E)]
        Sorcerer = Demolitionist | Arcanist,

        [MasteryInfo("https://image.ibb.co/eHUcJa/19_Purifier.png", 0xAC7899)]
        Purifier = Demolitionist | Inquisitor,

        [MasteryInfo("https://image.ibb.co/c08trv/20_Defiler.png", 0xB9AC80)]
        Defiler = Demolitionist | Necromancer,

        [MasteryInfo("https://image.ibb.co/nA0TPF/21_Witch_Hunter.png", 0x4A8892)]
        WitchHunter = Occultist | Nightblade,

        [MasteryInfo("https://image.ibb.co/kxHWda/22_Warlock.png", 0x26CA67)]
        Warlock = Occultist | Arcanist,

        [MasteryInfo("https://image.ibb.co/gettrv/23_Conjurer.png", 0x40A279)]
        Conjurer = Occultist | Shaman,

        [MasteryInfo("https://image.ibb.co/daENjF/24_Deceiver.png", 0x608E92)]
        Deceiver = Occultist | Inquisitor,

        [MasteryInfo("https://image.ibb.co/gqSKWv/25_Cabalist.png", 0x6CC279)]
        Cabalist = Occultist | Necromancer,

        [MasteryInfo("https://image.ibb.co/j48hjF/26_Spellbreaker.png", 0x24A7D4)]
        Spellbreaker = Nightblade | Arcanist,

        [MasteryInfo("https://image.ibb.co/eS7KWv/27_Trickster.png", 0x3D80E6)]
        Trickster = Nightblade | Shaman,

        [MasteryInfo("https://image.ibb.co/g8HmBv/28_Infiltrator.png", 0x5D6BFF)]
        Infiltrator = Nightblade | Inquisitor,

        [MasteryInfo("https://image.ibb.co/iTwPya/29_Reaper.png", 0x6AA0E6)]
        Reaper = Nightblade | Necromancer,

        [MasteryInfo("https://image.ibb.co/cR3RBv/30_Druid.png", 0x1AC0BA)]
        Druid = Arcanist | Shaman,

        [MasteryInfo("https://image.ibb.co/nq62jF/31_Mage_Hunter.png", 0x3AACD4)]
        MageHunter = Arcanist | Inquisitor,

        [MasteryInfo("https://image.ibb.co/nrsKWv/32_Spellbinder.png", 0x46E0BA)]
        Spellbinder = Arcanist | Necromancer,

        [MasteryInfo("https://image.ibb.co/ga62jF/33_Vindicator.png", 0x5384E6)]
        Vindicator = Shaman | Inquisitor,

        [MasteryInfo("https://image.ibb.co/eSCxJa/34_Ritualist.png", 0x60B9CC)]
        Ritualist = Shaman | Necromancer,

        [MasteryInfo("https://image.ibb.co/kPkTPF/35_Apostate.png", 0x7E9FE8)]
        Apostate = Inquisitor | Necromancer
    }
}
