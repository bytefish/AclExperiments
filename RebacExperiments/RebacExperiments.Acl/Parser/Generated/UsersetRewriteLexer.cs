//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/philipp/source/repos/bytefish/AclExperiments//RebacExperiments/RebacExperiments.Acl/Parser/UsersetRewrite.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace RebacExperiments.Acl.Parser.Generated {
#pragma warning disable 3021
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class UsersetRewriteLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, UNION=14, INTERSECT=15, EXCLUDE=16, 
		TUPLE_USERSET_NAMESPACE=17, TUPLE_USERSET_OBJECT=18, TUPLE_USERSET_RELATION=19, 
		STRING=20, SINGLE_LINE_COMMENT=21, MULTI_LINE_COMMENT=22, IDENTIFIER=23, 
		WS=24;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "UNION", "INTERSECT", "EXCLUDE", "TUPLE_USERSET_NAMESPACE", 
		"TUPLE_USERSET_OBJECT", "TUPLE_USERSET_RELATION", "STRING", "SINGLE_LINE_COMMENT", 
		"MULTI_LINE_COMMENT", "IDENTIFIER", "IDENTIFIER_START", "IDENTIFIER_PART", 
		"NEWLINE", "WS"
	};


	public UsersetRewriteLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public UsersetRewriteLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'name'", "':'", "'relation'", "'{'", "'}'", "'userset_rewrite'", 
		"'child'", "'computed_userset'", "'namespace'", "'object'", "'_this'", 
		"'tuple_to_userset'", "'tupleset'", "'union'", "'intersect'", "'exclude'", 
		"'$TUPLE_USERSET_NAMESPACE'", "'$TUPLE_USERSET_OBJECT'", "'$TUPLE_USERSET_RELATION'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, "UNION", "INTERSECT", "EXCLUDE", "TUPLE_USERSET_NAMESPACE", 
		"TUPLE_USERSET_OBJECT", "TUPLE_USERSET_RELATION", "STRING", "SINGLE_LINE_COMMENT", 
		"MULTI_LINE_COMMENT", "IDENTIFIER", "WS"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "UsersetRewrite.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static UsersetRewriteLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,24,331,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,1,0,1,0,1,0,1,0,
		1,0,1,1,1,1,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,3,1,3,1,4,1,4,1,5,1,
		5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,6,1,6,1,6,
		1,6,1,6,1,6,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,
		7,1,7,1,7,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,9,1,9,1,9,1,9,1,9,
		1,9,1,9,1,10,1,10,1,10,1,10,1,10,1,10,1,11,1,11,1,11,1,11,1,11,1,11,1,
		11,1,11,1,11,1,11,1,11,1,11,1,11,1,11,1,11,1,11,1,11,1,12,1,12,1,12,1,
		12,1,12,1,12,1,12,1,12,1,12,1,13,1,13,1,13,1,13,1,13,1,13,1,14,1,14,1,
		14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,15,1,15,1,15,1,15,1,15,1,15,1,
		15,1,15,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,
		16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,17,1,
		17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,
		17,1,17,1,17,1,17,1,17,1,17,1,17,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,
		18,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,
		18,1,18,1,18,1,19,1,19,5,19,261,8,19,10,19,12,19,264,9,19,1,19,1,19,1,
		19,5,19,269,8,19,10,19,12,19,272,9,19,1,19,3,19,275,8,19,1,20,1,20,1,20,
		1,20,5,20,281,8,20,10,20,12,20,284,9,20,1,20,1,20,3,20,288,8,20,1,20,1,
		20,1,21,1,21,1,21,1,21,5,21,296,8,21,10,21,12,21,299,9,21,1,21,1,21,1,
		21,1,21,1,21,1,22,1,22,5,22,308,8,22,10,22,12,22,311,9,22,1,23,3,23,314,
		8,23,1,24,1,24,3,24,318,8,24,1,25,1,25,1,25,3,25,323,8,25,1,26,4,26,326,
		8,26,11,26,12,26,327,1,26,1,26,2,282,297,0,27,1,1,3,2,5,3,7,4,9,5,11,6,
		13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,33,17,35,18,37,
		19,39,20,41,21,43,22,45,23,47,0,49,0,51,0,53,24,1,0,25,2,0,78,78,110,110,
		2,0,65,65,97,97,2,0,77,77,109,109,2,0,69,69,101,101,2,0,82,82,114,114,
		2,0,76,76,108,108,2,0,84,84,116,116,2,0,73,73,105,105,2,0,79,79,111,111,
		2,0,85,85,117,117,2,0,83,83,115,115,2,0,87,87,119,119,2,0,67,67,99,99,
		2,0,72,72,104,104,2,0,68,68,100,100,2,0,80,80,112,112,2,0,66,66,98,98,
		2,0,74,74,106,106,2,0,88,88,120,120,1,0,34,34,1,0,39,39,661,0,36,36,65,
		90,95,95,97,122,170,170,181,181,186,186,192,214,216,246,248,705,710,721,
		736,740,748,748,750,750,880,884,886,887,890,893,895,895,902,902,904,906,
		908,908,910,929,931,1013,1015,1153,1162,1327,1329,1366,1369,1369,1376,
		1416,1488,1514,1519,1522,1568,1610,1646,1647,1649,1747,1749,1749,1765,
		1766,1774,1775,1786,1788,1791,1791,1808,1808,1810,1839,1869,1957,1969,
		1969,1994,2026,2036,2037,2042,2042,2048,2069,2074,2074,2084,2084,2088,
		2088,2112,2136,2144,2154,2160,2183,2185,2190,2208,2249,2308,2361,2365,
		2365,2384,2384,2392,2401,2417,2432,2437,2444,2447,2448,2451,2472,2474,
		2480,2482,2482,2486,2489,2493,2493,2510,2510,2524,2525,2527,2529,2544,
		2545,2556,2556,2565,2570,2575,2576,2579,2600,2602,2608,2610,2611,2613,
		2614,2616,2617,2649,2652,2654,2654,2674,2676,2693,2701,2703,2705,2707,
		2728,2730,2736,2738,2739,2741,2745,2749,2749,2768,2768,2784,2785,2809,
		2809,2821,2828,2831,2832,2835,2856,2858,2864,2866,2867,2869,2873,2877,
		2877,2908,2909,2911,2913,2929,2929,2947,2947,2949,2954,2958,2960,2962,
		2965,2969,2970,2972,2972,2974,2975,2979,2980,2984,2986,2990,3001,3024,
		3024,3077,3084,3086,3088,3090,3112,3114,3129,3133,3133,3160,3162,3165,
		3165,3168,3169,3200,3200,3205,3212,3214,3216,3218,3240,3242,3251,3253,
		3257,3261,3261,3293,3294,3296,3297,3313,3314,3332,3340,3342,3344,3346,
		3386,3389,3389,3406,3406,3412,3414,3423,3425,3450,3455,3461,3478,3482,
		3505,3507,3515,3517,3517,3520,3526,3585,3632,3634,3635,3648,3654,3713,
		3714,3716,3716,3718,3722,3724,3747,3749,3749,3751,3760,3762,3763,3773,
		3773,3776,3780,3782,3782,3804,3807,3840,3840,3904,3911,3913,3948,3976,
		3980,4096,4138,4159,4159,4176,4181,4186,4189,4193,4193,4197,4198,4206,
		4208,4213,4225,4238,4238,4256,4293,4295,4295,4301,4301,4304,4346,4348,
		4680,4682,4685,4688,4694,4696,4696,4698,4701,4704,4744,4746,4749,4752,
		4784,4786,4789,4792,4798,4800,4800,4802,4805,4808,4822,4824,4880,4882,
		4885,4888,4954,4992,5007,5024,5109,5112,5117,5121,5740,5743,5759,5761,
		5786,5792,5866,5873,5880,5888,5905,5919,5937,5952,5969,5984,5996,5998,
		6000,6016,6067,6103,6103,6108,6108,6176,6264,6272,6276,6279,6312,6314,
		6314,6320,6389,6400,6430,6480,6509,6512,6516,6528,6571,6576,6601,6656,
		6678,6688,6740,6823,6823,6917,6963,6981,6988,7043,7072,7086,7087,7098,
		7141,7168,7203,7245,7247,7258,7293,7296,7304,7312,7354,7357,7359,7401,
		7404,7406,7411,7413,7414,7418,7418,7424,7615,7680,7957,7960,7965,7968,
		8005,8008,8013,8016,8023,8025,8025,8027,8027,8029,8029,8031,8061,8064,
		8116,8118,8124,8126,8126,8130,8132,8134,8140,8144,8147,8150,8155,8160,
		8172,8178,8180,8182,8188,8305,8305,8319,8319,8336,8348,8450,8450,8455,
		8455,8458,8467,8469,8469,8473,8477,8484,8484,8486,8486,8488,8488,8490,
		8493,8495,8505,8508,8511,8517,8521,8526,8526,8579,8580,11264,11492,11499,
		11502,11506,11507,11520,11557,11559,11559,11565,11565,11568,11623,11631,
		11631,11648,11670,11680,11686,11688,11694,11696,11702,11704,11710,11712,
		11718,11720,11726,11728,11734,11736,11742,11823,11823,12293,12294,12337,
		12341,12347,12348,12353,12438,12445,12447,12449,12538,12540,12543,12549,
		12591,12593,12686,12704,12735,12784,12799,13312,19903,19968,42124,42192,
		42237,42240,42508,42512,42527,42538,42539,42560,42606,42623,42653,42656,
		42725,42775,42783,42786,42888,42891,42954,42960,42961,42963,42963,42965,
		42969,42994,43009,43011,43013,43015,43018,43020,43042,43072,43123,43138,
		43187,43250,43255,43259,43259,43261,43262,43274,43301,43312,43334,43360,
		43388,43396,43442,43471,43471,43488,43492,43494,43503,43514,43518,43520,
		43560,43584,43586,43588,43595,43616,43638,43642,43642,43646,43695,43697,
		43697,43701,43702,43705,43709,43712,43712,43714,43714,43739,43741,43744,
		43754,43762,43764,43777,43782,43785,43790,43793,43798,43808,43814,43816,
		43822,43824,43866,43868,43881,43888,44002,44032,55203,55216,55238,55243,
		55291,63744,64109,64112,64217,64256,64262,64275,64279,64285,64285,64287,
		64296,64298,64310,64312,64316,64318,64318,64320,64321,64323,64324,64326,
		64433,64467,64829,64848,64911,64914,64967,65008,65019,65136,65140,65142,
		65276,65313,65338,65345,65370,65382,65470,65474,65479,65482,65487,65490,
		65495,65498,65500,65536,65547,65549,65574,65576,65594,65596,65597,65599,
		65613,65616,65629,65664,65786,66176,66204,66208,66256,66304,66335,66349,
		66368,66370,66377,66384,66421,66432,66461,66464,66499,66504,66511,66560,
		66717,66736,66771,66776,66811,66816,66855,66864,66915,66928,66938,66940,
		66954,66956,66962,66964,66965,66967,66977,66979,66993,66995,67001,67003,
		67004,67072,67382,67392,67413,67424,67431,67456,67461,67463,67504,67506,
		67514,67584,67589,67592,67592,67594,67637,67639,67640,67644,67644,67647,
		67669,67680,67702,67712,67742,67808,67826,67828,67829,67840,67861,67872,
		67897,67968,68023,68030,68031,68096,68096,68112,68115,68117,68119,68121,
		68149,68192,68220,68224,68252,68288,68295,68297,68324,68352,68405,68416,
		68437,68448,68466,68480,68497,68608,68680,68736,68786,68800,68850,68864,
		68899,69248,69289,69296,69297,69376,69404,69415,69415,69424,69445,69488,
		69505,69552,69572,69600,69622,69635,69687,69745,69746,69749,69749,69763,
		69807,69840,69864,69891,69926,69956,69956,69959,69959,69968,70002,70006,
		70006,70019,70066,70081,70084,70106,70106,70108,70108,70144,70161,70163,
		70187,70207,70208,70272,70278,70280,70280,70282,70285,70287,70301,70303,
		70312,70320,70366,70405,70412,70415,70416,70419,70440,70442,70448,70450,
		70451,70453,70457,70461,70461,70480,70480,70493,70497,70656,70708,70727,
		70730,70751,70753,70784,70831,70852,70853,70855,70855,71040,71086,71128,
		71131,71168,71215,71236,71236,71296,71338,71352,71352,71424,71450,71488,
		71494,71680,71723,71840,71903,71935,71942,71945,71945,71948,71955,71957,
		71958,71960,71983,71999,71999,72001,72001,72096,72103,72106,72144,72161,
		72161,72163,72163,72192,72192,72203,72242,72250,72250,72272,72272,72284,
		72329,72349,72349,72368,72440,72704,72712,72714,72750,72768,72768,72818,
		72847,72960,72966,72968,72969,72971,73008,73030,73030,73056,73061,73063,
		73064,73066,73097,73112,73112,73440,73458,73474,73474,73476,73488,73490,
		73523,73648,73648,73728,74649,74880,75075,77712,77808,77824,78895,78913,
		78918,82944,83526,92160,92728,92736,92766,92784,92862,92880,92909,92928,
		92975,92992,92995,93027,93047,93053,93071,93760,93823,93952,94026,94032,
		94032,94099,94111,94176,94177,94179,94179,94208,100343,100352,101589,101632,
		101640,110576,110579,110581,110587,110589,110590,110592,110882,110898,
		110898,110928,110930,110933,110933,110948,110951,110960,111355,113664,
		113770,113776,113788,113792,113800,113808,113817,119808,119892,119894,
		119964,119966,119967,119970,119970,119973,119974,119977,119980,119982,
		119993,119995,119995,119997,120003,120005,120069,120071,120074,120077,
		120084,120086,120092,120094,120121,120123,120126,120128,120132,120134,
		120134,120138,120144,120146,120485,120488,120512,120514,120538,120540,
		120570,120572,120596,120598,120628,120630,120654,120656,120686,120688,
		120712,120714,120744,120746,120770,120772,120779,122624,122654,122661,
		122666,122928,122989,123136,123180,123191,123197,123214,123214,123536,
		123565,123584,123627,124112,124139,124896,124902,124904,124907,124909,
		124910,124912,124926,124928,125124,125184,125251,125259,125259,126464,
		126467,126469,126495,126497,126498,126500,126500,126503,126503,126505,
		126514,126516,126519,126521,126521,126523,126523,126530,126530,126535,
		126535,126537,126537,126539,126539,126541,126543,126545,126546,126548,
		126548,126551,126551,126553,126553,126555,126555,126557,126557,126559,
		126559,126561,126562,126564,126564,126567,126570,126572,126578,126580,
		126583,126585,126588,126590,126590,126592,126601,126603,126619,126625,
		126627,126629,126633,126635,126651,131072,173791,173824,177977,177984,
		178205,178208,183969,183984,191456,194560,195101,196608,201546,201552,
		205743,436,0,48,57,95,95,178,179,185,185,188,190,768,879,1155,1161,1425,
		1469,1471,1471,1473,1474,1476,1477,1479,1479,1552,1562,1611,1641,1648,
		1648,1750,1756,1759,1764,1767,1768,1770,1773,1776,1785,1809,1809,1840,
		1866,1958,1968,1984,1993,2027,2035,2045,2045,2070,2073,2075,2083,2085,
		2087,2089,2093,2137,2139,2200,2207,2250,2273,2275,2307,2362,2364,2366,
		2383,2385,2391,2402,2403,2406,2415,2433,2435,2492,2492,2494,2500,2503,
		2504,2507,2509,2519,2519,2530,2531,2534,2543,2548,2553,2558,2558,2561,
		2563,2620,2620,2622,2626,2631,2632,2635,2637,2641,2641,2662,2673,2677,
		2677,2689,2691,2748,2748,2750,2757,2759,2761,2763,2765,2786,2787,2790,
		2799,2810,2815,2817,2819,2876,2876,2878,2884,2887,2888,2891,2893,2901,
		2903,2914,2915,2918,2927,2930,2935,2946,2946,3006,3010,3014,3016,3018,
		3021,3031,3031,3046,3058,3072,3076,3132,3132,3134,3140,3142,3144,3146,
		3149,3157,3158,3170,3171,3174,3183,3192,3198,3201,3203,3260,3260,3262,
		3268,3270,3272,3274,3277,3285,3286,3298,3299,3302,3311,3315,3315,3328,
		3331,3387,3388,3390,3396,3398,3400,3402,3405,3415,3422,3426,3427,3430,
		3448,3457,3459,3530,3530,3535,3540,3542,3542,3544,3551,3558,3567,3570,
		3571,3633,3633,3636,3642,3655,3662,3664,3673,3761,3761,3764,3772,3784,
		3790,3792,3801,3864,3865,3872,3891,3893,3893,3895,3895,3897,3897,3902,
		3903,3953,3972,3974,3975,3981,3991,3993,4028,4038,4038,4139,4158,4160,
		4169,4182,4185,4190,4192,4194,4196,4199,4205,4209,4212,4226,4237,4239,
		4253,4957,4959,4969,4988,5870,5872,5906,5909,5938,5940,5970,5971,6002,
		6003,6068,6099,6109,6109,6112,6121,6128,6137,6155,6157,6159,6169,6277,
		6278,6313,6313,6432,6443,6448,6459,6470,6479,6608,6618,6679,6683,6741,
		6750,6752,6780,6783,6793,6800,6809,6832,6862,6912,6916,6964,6980,6992,
		7001,7019,7027,7040,7042,7073,7085,7088,7097,7142,7155,7204,7223,7232,
		7241,7248,7257,7376,7378,7380,7400,7405,7405,7412,7412,7415,7417,7616,
		7679,8204,8205,8255,8256,8276,8276,8304,8304,8308,8313,8320,8329,8400,
		8432,8528,8578,8581,8585,9312,9371,9450,9471,10102,10131,11503,11505,11517,
		11517,11647,11647,11744,11775,12295,12295,12321,12335,12344,12346,12441,
		12442,12690,12693,12832,12841,12872,12879,12881,12895,12928,12937,12977,
		12991,42528,42537,42607,42610,42612,42621,42654,42655,42726,42737,43010,
		43010,43014,43014,43019,43019,43043,43047,43052,43052,43056,43061,43136,
		43137,43188,43205,43216,43225,43232,43249,43263,43273,43302,43309,43335,
		43347,43392,43395,43443,43456,43472,43481,43493,43493,43504,43513,43561,
		43574,43587,43587,43596,43597,43600,43609,43643,43645,43696,43696,43698,
		43700,43703,43704,43710,43711,43713,43713,43755,43759,43765,43766,44003,
		44010,44012,44013,44016,44025,64286,64286,65024,65039,65056,65071,65075,
		65076,65101,65103,65296,65305,65343,65343,65799,65843,65856,65912,65930,
		65931,66045,66045,66272,66299,66336,66339,66369,66369,66378,66378,66422,
		66426,66513,66517,66720,66729,67672,67679,67705,67711,67751,67759,67835,
		67839,67862,67867,68028,68029,68032,68047,68050,68095,68097,68099,68101,
		68102,68108,68111,68152,68154,68159,68168,68221,68222,68253,68255,68325,
		68326,68331,68335,68440,68447,68472,68479,68521,68527,68858,68863,68900,
		68903,68912,68921,69216,69246,69291,69292,69373,69375,69405,69414,69446,
		69460,69506,69509,69573,69579,69632,69634,69688,69702,69714,69744,69747,
		69748,69759,69762,69808,69818,69826,69826,69872,69881,69888,69890,69927,
		69940,69942,69951,69957,69958,70003,70003,70016,70018,70067,70080,70089,
		70092,70094,70105,70113,70132,70188,70199,70206,70206,70209,70209,70367,
		70378,70384,70393,70400,70403,70459,70460,70462,70468,70471,70472,70475,
		70477,70487,70487,70498,70499,70502,70508,70512,70516,70709,70726,70736,
		70745,70750,70750,70832,70851,70864,70873,71087,71093,71096,71104,71132,
		71133,71216,71232,71248,71257,71339,71351,71360,71369,71453,71467,71472,
		71483,71724,71738,71904,71922,71984,71989,71991,71992,71995,71998,72000,
		72000,72002,72003,72016,72025,72145,72151,72154,72160,72164,72164,72193,
		72202,72243,72249,72251,72254,72263,72263,72273,72283,72330,72345,72751,
		72758,72760,72767,72784,72812,72850,72871,72873,72886,73009,73014,73018,
		73018,73020,73021,73023,73029,73031,73031,73040,73049,73098,73102,73104,
		73105,73107,73111,73120,73129,73459,73462,73472,73473,73475,73475,73524,
		73530,73534,73538,73552,73561,73664,73684,74752,74862,78912,78912,78919,
		78933,92768,92777,92864,92873,92912,92916,92976,92982,93008,93017,93019,
		93025,93824,93846,94031,94031,94033,94087,94095,94098,94180,94180,94192,
		94193,113821,113822,118528,118573,118576,118598,119141,119145,119149,119154,
		119163,119170,119173,119179,119210,119213,119362,119364,119488,119507,
		119520,119539,119648,119672,120782,120831,121344,121398,121403,121452,
		121461,121461,121476,121476,121499,121503,121505,121519,122880,122886,
		122888,122904,122907,122913,122915,122916,122918,122922,123023,123023,
		123184,123190,123200,123209,123566,123566,123628,123641,124140,124153,
		125127,125142,125252,125258,125264,125273,126065,126123,126125,126127,
		126129,126132,126209,126253,126255,126269,127232,127244,130032,130041,
		917760,917999,3,0,10,10,13,13,8232,8233,6,0,9,10,13,13,32,32,160,160,8195,
		8195,65279,65279,337,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,0,0,0,0,
		9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,0,19,1,0,
		0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,29,1,0,0,0,0,
		31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,0,0,0,0,39,1,0,0,0,0,41,1,
		0,0,0,0,43,1,0,0,0,0,45,1,0,0,0,0,53,1,0,0,0,1,55,1,0,0,0,3,60,1,0,0,0,
		5,62,1,0,0,0,7,71,1,0,0,0,9,73,1,0,0,0,11,75,1,0,0,0,13,91,1,0,0,0,15,
		97,1,0,0,0,17,114,1,0,0,0,19,124,1,0,0,0,21,131,1,0,0,0,23,137,1,0,0,0,
		25,154,1,0,0,0,27,163,1,0,0,0,29,169,1,0,0,0,31,179,1,0,0,0,33,187,1,0,
		0,0,35,212,1,0,0,0,37,234,1,0,0,0,39,274,1,0,0,0,41,276,1,0,0,0,43,291,
		1,0,0,0,45,305,1,0,0,0,47,313,1,0,0,0,49,317,1,0,0,0,51,322,1,0,0,0,53,
		325,1,0,0,0,55,56,7,0,0,0,56,57,7,1,0,0,57,58,7,2,0,0,58,59,7,3,0,0,59,
		2,1,0,0,0,60,61,5,58,0,0,61,4,1,0,0,0,62,63,7,4,0,0,63,64,7,3,0,0,64,65,
		7,5,0,0,65,66,7,1,0,0,66,67,7,6,0,0,67,68,7,7,0,0,68,69,7,8,0,0,69,70,
		7,0,0,0,70,6,1,0,0,0,71,72,5,123,0,0,72,8,1,0,0,0,73,74,5,125,0,0,74,10,
		1,0,0,0,75,76,7,9,0,0,76,77,7,10,0,0,77,78,7,3,0,0,78,79,7,4,0,0,79,80,
		7,10,0,0,80,81,7,3,0,0,81,82,7,6,0,0,82,83,5,95,0,0,83,84,7,4,0,0,84,85,
		7,3,0,0,85,86,7,11,0,0,86,87,7,4,0,0,87,88,7,7,0,0,88,89,7,6,0,0,89,90,
		7,3,0,0,90,12,1,0,0,0,91,92,7,12,0,0,92,93,7,13,0,0,93,94,7,7,0,0,94,95,
		7,5,0,0,95,96,7,14,0,0,96,14,1,0,0,0,97,98,7,12,0,0,98,99,7,8,0,0,99,100,
		7,2,0,0,100,101,7,15,0,0,101,102,7,9,0,0,102,103,7,6,0,0,103,104,7,3,0,
		0,104,105,7,14,0,0,105,106,5,95,0,0,106,107,7,9,0,0,107,108,7,10,0,0,108,
		109,7,3,0,0,109,110,7,4,0,0,110,111,7,10,0,0,111,112,7,3,0,0,112,113,7,
		6,0,0,113,16,1,0,0,0,114,115,7,0,0,0,115,116,7,1,0,0,116,117,7,2,0,0,117,
		118,7,3,0,0,118,119,7,10,0,0,119,120,7,15,0,0,120,121,7,1,0,0,121,122,
		7,12,0,0,122,123,7,3,0,0,123,18,1,0,0,0,124,125,7,8,0,0,125,126,7,16,0,
		0,126,127,7,17,0,0,127,128,7,3,0,0,128,129,7,12,0,0,129,130,7,6,0,0,130,
		20,1,0,0,0,131,132,5,95,0,0,132,133,7,6,0,0,133,134,7,13,0,0,134,135,7,
		7,0,0,135,136,7,10,0,0,136,22,1,0,0,0,137,138,7,6,0,0,138,139,7,9,0,0,
		139,140,7,15,0,0,140,141,7,5,0,0,141,142,7,3,0,0,142,143,5,95,0,0,143,
		144,7,6,0,0,144,145,7,8,0,0,145,146,5,95,0,0,146,147,7,9,0,0,147,148,7,
		10,0,0,148,149,7,3,0,0,149,150,7,4,0,0,150,151,7,10,0,0,151,152,7,3,0,
		0,152,153,7,6,0,0,153,24,1,0,0,0,154,155,7,6,0,0,155,156,7,9,0,0,156,157,
		7,15,0,0,157,158,7,5,0,0,158,159,7,3,0,0,159,160,7,10,0,0,160,161,7,3,
		0,0,161,162,7,6,0,0,162,26,1,0,0,0,163,164,7,9,0,0,164,165,7,0,0,0,165,
		166,7,7,0,0,166,167,7,8,0,0,167,168,7,0,0,0,168,28,1,0,0,0,169,170,7,7,
		0,0,170,171,7,0,0,0,171,172,7,6,0,0,172,173,7,3,0,0,173,174,7,4,0,0,174,
		175,7,10,0,0,175,176,7,3,0,0,176,177,7,12,0,0,177,178,7,6,0,0,178,30,1,
		0,0,0,179,180,7,3,0,0,180,181,7,18,0,0,181,182,7,12,0,0,182,183,7,5,0,
		0,183,184,7,9,0,0,184,185,7,14,0,0,185,186,7,3,0,0,186,32,1,0,0,0,187,
		188,5,36,0,0,188,189,7,6,0,0,189,190,7,9,0,0,190,191,7,15,0,0,191,192,
		7,5,0,0,192,193,7,3,0,0,193,194,5,95,0,0,194,195,7,9,0,0,195,196,7,10,
		0,0,196,197,7,3,0,0,197,198,7,4,0,0,198,199,7,10,0,0,199,200,7,3,0,0,200,
		201,7,6,0,0,201,202,5,95,0,0,202,203,7,0,0,0,203,204,7,1,0,0,204,205,7,
		2,0,0,205,206,7,3,0,0,206,207,7,10,0,0,207,208,7,15,0,0,208,209,7,1,0,
		0,209,210,7,12,0,0,210,211,7,3,0,0,211,34,1,0,0,0,212,213,5,36,0,0,213,
		214,7,6,0,0,214,215,7,9,0,0,215,216,7,15,0,0,216,217,7,5,0,0,217,218,7,
		3,0,0,218,219,5,95,0,0,219,220,7,9,0,0,220,221,7,10,0,0,221,222,7,3,0,
		0,222,223,7,4,0,0,223,224,7,10,0,0,224,225,7,3,0,0,225,226,7,6,0,0,226,
		227,5,95,0,0,227,228,7,8,0,0,228,229,7,16,0,0,229,230,7,17,0,0,230,231,
		7,3,0,0,231,232,7,12,0,0,232,233,7,6,0,0,233,36,1,0,0,0,234,235,5,36,0,
		0,235,236,7,6,0,0,236,237,7,9,0,0,237,238,7,15,0,0,238,239,7,5,0,0,239,
		240,7,3,0,0,240,241,5,95,0,0,241,242,7,9,0,0,242,243,7,10,0,0,243,244,
		7,3,0,0,244,245,7,4,0,0,245,246,7,10,0,0,246,247,7,3,0,0,247,248,7,6,0,
		0,248,249,5,95,0,0,249,250,7,4,0,0,250,251,7,3,0,0,251,252,7,5,0,0,252,
		253,7,1,0,0,253,254,7,6,0,0,254,255,7,7,0,0,255,256,7,8,0,0,256,257,7,
		0,0,0,257,38,1,0,0,0,258,262,5,34,0,0,259,261,8,19,0,0,260,259,1,0,0,0,
		261,264,1,0,0,0,262,260,1,0,0,0,262,263,1,0,0,0,263,265,1,0,0,0,264,262,
		1,0,0,0,265,275,5,34,0,0,266,270,5,39,0,0,267,269,8,20,0,0,268,267,1,0,
		0,0,269,272,1,0,0,0,270,268,1,0,0,0,270,271,1,0,0,0,271,273,1,0,0,0,272,
		270,1,0,0,0,273,275,5,39,0,0,274,258,1,0,0,0,274,266,1,0,0,0,275,40,1,
		0,0,0,276,277,5,47,0,0,277,278,5,47,0,0,278,282,1,0,0,0,279,281,9,0,0,
		0,280,279,1,0,0,0,281,284,1,0,0,0,282,283,1,0,0,0,282,280,1,0,0,0,283,
		287,1,0,0,0,284,282,1,0,0,0,285,288,3,51,25,0,286,288,5,0,0,1,287,285,
		1,0,0,0,287,286,1,0,0,0,288,289,1,0,0,0,289,290,6,20,0,0,290,42,1,0,0,
		0,291,292,5,47,0,0,292,293,5,42,0,0,293,297,1,0,0,0,294,296,9,0,0,0,295,
		294,1,0,0,0,296,299,1,0,0,0,297,298,1,0,0,0,297,295,1,0,0,0,298,300,1,
		0,0,0,299,297,1,0,0,0,300,301,5,42,0,0,301,302,5,47,0,0,302,303,1,0,0,
		0,303,304,6,21,0,0,304,44,1,0,0,0,305,309,3,47,23,0,306,308,3,49,24,0,
		307,306,1,0,0,0,308,311,1,0,0,0,309,307,1,0,0,0,309,310,1,0,0,0,310,46,
		1,0,0,0,311,309,1,0,0,0,312,314,7,21,0,0,313,312,1,0,0,0,314,48,1,0,0,
		0,315,318,3,47,23,0,316,318,7,22,0,0,317,315,1,0,0,0,317,316,1,0,0,0,318,
		50,1,0,0,0,319,320,5,13,0,0,320,323,5,10,0,0,321,323,7,23,0,0,322,319,
		1,0,0,0,322,321,1,0,0,0,323,52,1,0,0,0,324,326,7,24,0,0,325,324,1,0,0,
		0,326,327,1,0,0,0,327,325,1,0,0,0,327,328,1,0,0,0,328,329,1,0,0,0,329,
		330,6,26,0,0,330,54,1,0,0,0,12,0,262,270,274,282,287,297,309,313,317,322,
		327,1,6,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace RebacExperiments.Acl.Parser.Generated
