import torch


def generate_wav(text, path="./Assets/tts.wav", speaker="aidar", language="ru", model_id="v4_ru"):
    torch.hub.set_dir("./../models/torch")
    model, _ = torch.hub.load("snakers4/silero-models", "silero_tts", language, model_id)
    
    text = '<speak>' + text + '</speak>'

    model.save_wav(ssml_text=text,
                         speaker=speaker,
                         sample_rate=48000,
                         put_accent=True,
                         put_yo=True,
                         audio_path=path)

    return model