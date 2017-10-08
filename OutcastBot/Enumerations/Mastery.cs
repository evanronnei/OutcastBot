using System;

namespace OutcastBot.Enumerations
{
    [Flags]
    public enum Mastery
    {
        [MasteryInfo("https://i.imgur.com/65EvALc.png", 0xF5D166)]
        Soldier = 1 << 0,

        [MasteryInfo("https://i.imgur.com/5012yyB.png", 0xE68033)]
        Demolitionist = 1 << 1,

        [MasteryInfo("https://i.imgur.com/kSYc9Ag.png", 0x4DAB26)]
        Occultist = 1 << 2,

        [MasteryInfo("https://i.imgur.com/lpFiVaM.png", 0x4766FF)]
        Nightblade = 1 << 3,

        [MasteryInfo("https://i.imgur.com/IQS3LZO.png", 0x00E8A8)]
        Arcanist = 1 << 4,

        [MasteryInfo("https://i.imgur.com/lMRSEej.png", 0x3399CC)]
        Shaman = 1 << 5,

        [MasteryInfo("https://i.imgur.com/8h8CCfB.png", 0x7370FF)]
        Inquisitor = 1 << 6,

        [MasteryInfo("https://i.imgur.com/36YTOSc.png", 0x8CD9CC)]
        Necromancer = 1 << 7,

        [MasteryInfo("https://i.imgur.com/zXqMyxy.png", 0xEEA84C)]
        Commando = Soldier | Demolitionist,

        [MasteryInfo("https://i.imgur.com/6q6w7kN.png", 0xA1BE46)]
        Witchblade = Soldier | Occultist,

        [MasteryInfo("https://i.imgur.com/efXIkNx.png", 0x9E9CB2)]
        Blademaster = Soldier | Nightblade,

        [MasteryInfo("https://i.imgur.com/4TL7OMw.png", 0x7ADC87)]
        Battlemage = Soldier | Arcanist,

        [MasteryInfo("https://i.imgur.com/clfBgEl.png", 0x94B599)]
        Warder = Soldier | Shaman,

        [MasteryInfo("https://i.imgur.com/xNcP1ZZ.png", 0xB4A0B2)]
        Tactician = Soldier | Inquisitor,

        [MasteryInfo("https://i.imgur.com/0KnCKuT.png", 0xC0D599)]
        DeathKnight = Soldier | Necromancer,

        [MasteryInfo("https://i.imgur.com/B8YIKR0.png", 0x9A962C)]
        Pyromancer = Demolitionist | Occultist,

        [MasteryInfo("https://i.imgur.com/3487XYk.png", 0x967399)]
        Saboteur = Demolitionist | Nightblade,

        [MasteryInfo("https://i.imgur.com/LuHsqsB.png", 0x73B46E)]
        Sorcerer = Demolitionist | Arcanist,

        [MasteryInfo("https://i.imgur.com/lsHA1g3.png", 0x8C8C80)]
        Elementalist = Demolitionist | Shaman,

        [MasteryInfo("https://i.imgur.com/vqE8IiI.png", 0xAC7899)]
        Purifier = Demolitionist | Inquisitor,

        [MasteryInfo("https://i.imgur.com/mIFlYT1.png", 0xB9AC80)]
        Defiler = Demolitionist | Necromancer,

        [MasteryInfo("https://i.imgur.com/TDZEgQG.png", 0x4A8892)]
        WitchHunter = Occultist | Nightblade,

        [MasteryInfo("https://i.imgur.com/GjZkz94.png", 0x26CA67)]
        Warlock = Occultist | Arcanist,

        [MasteryInfo("https://i.imgur.com/iWAIedV.png", 0x40A279)]
        Conjurer = Occultist | Shaman,

        [MasteryInfo("https://i.imgur.com/c2FwslH.png", 0x608E92)]
        Deceiver = Occultist | Inquisitor,

        [MasteryInfo("https://i.imgur.com/ojbBALn.png", 0x6CC279)]
        Cabalist = Occultist | Necromancer,

        [MasteryInfo("https://i.imgur.com/InbSXeO.png", 0x24A7D4)]
        Spellbreaker = Nightblade | Arcanist,

        [MasteryInfo("https://i.imgur.com/V1jvsWN.png", 0x3D80E6)]
        Trickster = Nightblade | Shaman,

        [MasteryInfo("https://i.imgur.com/KdekTMW.png", 0x5D6BFF)]
        Infiltrator = Nightblade | Inquisitor,

        [MasteryInfo("https://i.imgur.com/fcC2CoH.png", 0x6AA0E6)]
        Reaper = Nightblade | Necromancer,

        [MasteryInfo("https://i.imgur.com/NdGuSlI.png", 0x1AC0BA)]
        Druid = Arcanist | Shaman,

        [MasteryInfo("https://i.imgur.com/8XiEvM3.png", 0x3AACD4)]
        MageHunter = Arcanist | Inquisitor,

        [MasteryInfo("https://i.imgur.com/GAWKnhi.png", 0x46E0BA)]
        Spellbinder = Arcanist | Necromancer,

        [MasteryInfo("https://i.imgur.com/6MqKZJY.png", 0x5384E6)]
        Vindicator = Shaman | Inquisitor,

        [MasteryInfo("https://i.imgur.com/sZRa1Rs.png", 0x60B9CC)]
        Ritualist = Shaman | Necromancer,

        [MasteryInfo("https://i.imgur.com/MFKOlKj.png", 0x7E9FE8)]
        Apostate = Inquisitor | Necromancer
    }
}
