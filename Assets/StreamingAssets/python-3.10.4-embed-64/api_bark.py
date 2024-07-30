from bark import SAMPLE_RATE, generate_audio, preload_models
from scipy.io.wavfile import write as write_wav


def generate_wav(answer, path="./Assets/bark_tts.wav", history_prompt="v2/ru_speaker_5"):
    preload_models()

    text_prompt = answer
    audio_array = generate_audio(text_prompt, history_prompt=history_prompt)
    write_wav(path, SAMPLE_RATE, audio_array)
