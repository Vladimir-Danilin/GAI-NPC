import os, asyncio
from together import AsyncTogether

async def async_chat_completion(messages):
    async_client = AsyncTogether(api_key="804c67bbc002570c94c2e09da5ce7d1b575f852407fce7519c8ddd54be5aa5d6")
    tasks = [
        async_client.chat.completions.create(
            model="mistralai/Mixtral-8x7B-Instruct-v0.1",
            messages=[{"role": "user", "content": message}],
        )
        for message in messages
    ]
    responses = await asyncio.gather(*tasks)

    return responses[0].choices[0].message.content


def generate_text(messages):
    return asyncio.run(async_chat_completion(messages))