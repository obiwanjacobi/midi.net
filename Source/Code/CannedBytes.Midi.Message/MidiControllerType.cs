namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Specifies constants defining controller types.
    /// </summary>
    public enum MidiControllerType
    {
        /// <summary>
        /// The Bank Select coarse.
        /// </summary>
        BankSelect = 0,

        /// <summary>
        /// The Modulation Wheel coarse.
        /// </summary>
        ModulationWheel = 1,

        /// <summary>
        /// The Breath Control coarse.
        /// </summary>
        BreathControl = 2,

        /// <summary>
        /// The Foot Pedal coarse.
        /// </summary>
        FootPedal = 4,

        /// <summary>
        /// The Portamento Time coarse.
        /// </summary>
        PortamentoTime = 5,

        /// <summary>
        /// The Data Entry Slider coarse.
        /// </summary>
        DataEntrySlider = 6,

        /// <summary>
        /// The Volume coarse.
        /// </summary>
        Volume = 7,

        /// <summary>
        /// The Balance coarse.
        /// </summary>
        Balance = 8,

        /// <summary>
        /// The Pan position coarse.
        /// </summary>
        Pan = 10,

        /// <summary>
        /// The Expression coarse.
        /// </summary>
        Expression = 11,

        /// <summary>
        /// The Effect Control 1 coarse.
        /// </summary>
        EffectControl1 = 12,

        /// <summary>
        /// The Effect Control 2 coarse.
        /// </summary>
        EffectControl2 = 13,

        /// <summary>
        /// The General Puprose Slider 1
        /// </summary>
        GeneralPurposeSlider1 = 16,

        /// <summary>
        /// The General Puprose Slider 2
        /// </summary>
        GeneralPurposeSlider2 = 17,

        /// <summary>
        /// The General Puprose Slider 3
        /// </summary>
        GeneralPurposeSlider3 = 18,

        /// <summary>
        /// The General Puprose Slider 4
        /// </summary>
        GeneralPurposeSlider4 = 19,

        /// <summary>
        /// The Bank Select fine.
        /// </summary>
        BankSelectFine = 32,

        /// <summary>
        /// The Modulation Wheel fine.
        /// </summary>
        ModulationWheelFine = 33,

        /// <summary>
        /// The Breath Control fine.
        /// </summary>
        BreathControlFine = 34,

        /// <summary>
        /// The Foot Pedal fine.
        /// </summary>
        FootPedalFine = 36,

        /// <summary>
        /// The Portamento Time fine.
        /// </summary>
        PortamentoTimeFine = 37,

        /// <summary>
        /// The Data Entry Slider fine.
        /// </summary>
        DataEntrySliderFine = 38,

        /// <summary>
        /// The Volume fine.
        /// </summary>
        VolumeFine = 39,

        /// <summary>
        /// The Balance fine.
        /// </summary>
        BalanceFine = 40,

        /// <summary>
        /// The Pan position fine.
        /// </summary>
        PanFine = 42,

        /// <summary>
        /// The Expression fine.
        /// </summary>
        ExpressionFine = 43,

        /// <summary>
        /// The Effect Control 1 fine.
        /// </summary>
        EffectControl1Fine = 44,

        /// <summary>
        /// The Effect Control 2 fine.
        /// </summary>
        EffectControl2Fine = 45,

        /// <summary>
        /// The Hold Pedal.
        /// </summary>
        HoldPedal = 64,

        /// <summary>
        /// The Portamento.
        /// </summary>
        Portamento = 65,

        /// <summary>
        /// The Sustenuto Pedal.
        /// </summary>
        SustenutoPedal = 66,

        /// <summary>
        /// The Soft Pedal.
        /// </summary>
        SoftPedal = 67,

        /// <summary>
        /// The Legato Pedal.
        /// </summary>
        LegatoPedal = 68,

        /// <summary>
        /// The Hold Pedal 2.
        /// </summary>
        HoldPedal2 = 69,

        /// <summary>
        /// The Sound Variation.
        /// </summary>
        SoundVariation = 70,

        /// <summary>
        /// The Sound Timbre.
        /// </summary>
        SoundTimbre = 71,

        /// <summary>
        /// The Sound Release Time.
        /// </summary>
        SoundReleaseTime = 72,

        /// <summary>
        /// The Sound Attack Time.
        /// </summary>
        SoundAttackTime = 73,

        /// <summary>
        /// The Sound Brightness.
        /// </summary>
        SoundBrightness = 74,

        /// <summary>
        /// The Sound Control 6.
        /// </summary>
        SoundControl6 = 75,

        /// <summary>
        /// The Sound Control 7.
        /// </summary>
        SoundControl7 = 76,

        /// <summary>
        /// The Sound Control 8.
        /// </summary>
        SoundControl8 = 77,

        /// <summary>
        /// The Sound Control 9.
        /// </summary>
        SoundControl9 = 78,

        /// <summary>
        /// The Sound Control 10.
        /// </summary>
        SoundControl10 = 79,

        /// <summary>
        /// The General Purpose Button 1.
        /// </summary>
        GeneralPurposeButton1 = 80,

        /// <summary>
        /// The General Purpose Button 2.
        /// </summary>
        GeneralPurposeButton2 = 81,

        /// <summary>
        /// The General Purpose Button 3.
        /// </summary>
        GeneralPurposeButton3 = 82,

        /// <summary>
        /// The General Purpose Button 4.
        /// </summary>
        GeneralPurposeButton4 = 83,

        /// <summary>
        /// The Effects Level.
        /// </summary>
        EffectsLevel = 91,

        /// <summary>
        /// The Tremelo Level.
        /// </summary>
        TremeloLevel = 92,

        /// <summary>
        /// The Chorus Level.
        /// </summary>
        ChorusLevel = 93,

        /// <summary>
        /// The Celeste Level.
        /// </summary>
        CelesteLevel = 94,

        /// <summary>
        /// The Phaser Level.
        /// </summary>
        PhaserLevel = 95,

        /// <summary>
        /// The Data Button Increment.
        /// </summary>
        DataButtonIncrement = 96,

        /// <summary>
        /// The Data Button Decrement.
        /// </summary>
        DataButtonDecrement = 97,

        /// <summary>
        /// The Nonregistered Parameter Fine.
        /// </summary>
        NonRegisteredParameterFine = 98,

        /// <summary>
        /// The Nonregistered Parameter Coarse.
        /// </summary>
        NonRegisteredParameterCoarse = 99,

        /// <summary>
        /// The Registered Parameter Fine.
        /// </summary>
        RegisteredParameterFine = 100,

        /// <summary>
        /// The Registered Parameter Coarse.
        /// </summary>
        RegisteredParameterCoarse = 101,

        /// <summary>
        /// The All Sound Off.
        /// </summary>
        AllSoundOff = 120,

        /// <summary>
        /// The All Controllers Off.
        /// </summary>
        AllControllersOff = 121,

        /// <summary>
        /// The Local Keyboard.
        /// </summary>
        LocalKeyboard = 122,

        /// <summary>
        /// The All Notes Off.
        /// </summary>
        AllNotesOff = 123,

        /// <summary>
        /// The Omni Mode Off.
        /// </summary>
        OmniModeOff = 124,

        /// <summary>
        /// The Omni Mode On.
        /// </summary>
        OmniModeOn = 125,

        /// <summary>
        /// The Mono Operation.
        /// </summary>
        MonoOperation = 126,

        /// <summary>
        /// The Poly Operation.
        /// </summary>
        PolyOperation = 127
    }
}