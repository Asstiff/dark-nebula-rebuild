# Audio audit and Unity encoding impact

Date: 2026-11-04. Unity: 6000.4.11f1. Assessment target: Android.

## Findings

All 116 project clips use Vorbis at quality **0.0**. Android has no per-platform override. Sample rates are preserved and no forced mono conversion is configured. Consequently the existing rebuild adds a lossy encoding stage, including for already-compressed MP3 sources.

All project audio files are byte-identical to the recovered Android files. All 83 Episode Two files also match byte payloads from the compared iOS package. Matched Episode One iOS files have the same codec, sample rate, channels and bitrate; some compressed payloads differ, so they are not claimed to sound identical. No higher-specification substitute was found among these matched sources.

| Project | Files | Formats | Sample rates |
| --- | ---: | --- | --- |
| Episode One | 33 | 18 MP3, 15 WAV | 27 at 22,050 Hz; 6 at 44,100 Hz |
| Episode Two | 83 | 82 MP3, 1 WAV | 81 at 22,050 Hz; 1 at 44,100 Hz; 1 at 48,000 Hz |

All source MP3 files report 64 kbps. Increasing their sample rate does not restore missing source detail. Five current Episode One names are absent from the compared iOS package: button2, magnet1, magnet2, magnet3 and shop_buy.

## Controlled encoding assessment

Nine fixtures were encoded into 27 Android AssetBundles: current MP3 inputs, original PCM effects and PCM-decoded copies of two MP3 sources. Each was imported with Vorbis quality 0, Vorbis quality 1 or PCM. Actual default and Android settings were queried after import. Quality-1 cases explicitly override Android settings.

Each encoded bundle was extracted and decoded. Comparisons use the matching Unity PCM import as reference, preserve gain and channels, and check for timing offset. The reported fixtures required zero-sample alignment adjustment. SNR and normalized waveform error are objective measurements, not listening scores, and do not measure loss already present in the source MP3.

| Controlled PCM input | Vorbis setting | Encoded payload | SNR vs PCM | Normalized waveform RMSE |
| --- | --- | ---: | ---: | ---: |
| Forest music, decoded from source MP3 | 0 | 898.94 KiB | 16.92 dB | 14.25% |
| Forest music, same decoded input | 1 | 3,355.09 KiB | 33.31 dB | 2.16% |
| Scan effect, decoded from source MP3 | 0 | 8.19 KiB | 20.83 dB | 9.09% |
| Scan effect, same decoded input | 1 | 16.47 KiB | 30.61 dB | 2.95% |
| Original PCM button effect | 0 | 2.34 KiB | 26.12 dB | 4.95% |
| Original PCM button effect | 1 | 3.78 KiB | 37.85 dB | 1.28% |
| Original PCM tick | 0 | 0.56 KiB | 21.55 dB | 8.37% |
| Original PCM tick | 1 | 0.94 KiB | 30.67 dB | 2.93% |

**Important observed behavior:** when importing the original MP3 files directly, changing quality 0 to 1 produced identical decoded output for forest music, credits music, the scan effect and the turret effect, despite the importer reporting quality 1 and an Android override. Raising only the quality field is therefore not a verified remedy for this project's MP3 inputs. Converting the same recordings to PCM first made quality 1 effective in this controlled test. This observation is specific to these inputs and this Unity version, not a general assertion about every Unity MP3 import.

The direct-MP3 forest import was 898.25 KiB with SNR 16.93 dB relative to its PCM baseline; direct-MP3 scan was 8.06 KiB with SNR 20.87 dB. These results show additional error after the original MP3 decode.

## Tradeoffs

- PCM import avoids the measured Vorbis error relative to the PCM reference, at a larger data size. The forest fixture's PCM payload is 15,239.38 KiB and the scan fixture's is 132.88 KiB.
- Decoding MP3 to PCM and then using verified high-quality Vorbis substantially reduces the added error in these fixtures while remaining smaller than final PCM. It does not improve the original recording.
- The approximate total decoded 16-bit PCM data would be 92.25 MiB for Episode One and 153.73 MiB for Episode Two. These are raw data estimates, not final APK sizes.
- Long music should retain streaming behavior if this is implemented. Device CPU, memory, latency and listening acceptance were not measured in this assessment.

The assessment changed no original source clips or their import settings. Its staging assets and temporary editor script were removed. Buildable game audio therefore remains at the existing settings until a separate improvement is implemented.

Numerical records, spectrum plots and synchronized fixed-level listening excerpts accompany this report under validation/audio-evaluation. Raw Android/iOS source comparisons and importer queries are under validation/audio.

[Spectrum comparison](validation/audio-evaluation/compression-spectrum.png), [full measurements](validation/audio-evaluation/metrics.json), [verified Android importer settings for each fixture](validation/audio-evaluation/encoded-fixtures.json).

Fixed-level forest excerpts, using the same decoded PCM input: [PCM reference](validation/audio-evaluation/listening/forest_decoded_pcm.wav), [quality 0](validation/audio-evaluation/listening/forest_decoded_q0.wav), [quality 1](validation/audio-evaluation/listening/forest_decoded_q1.wav).

Fixed-level scan excerpts: [PCM reference](validation/audio-evaluation/listening/scan_decoded_pcm.wav), [quality 0](validation/audio-evaluation/listening/scan_decoded_q0.wav), [quality 1](validation/audio-evaluation/listening/scan_decoded_q1.wav).

Unity's API describes [audio compression quality as a 0–1 value](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioImporterSampleSettings-quality.html).
