from together import Together


def call_client():
    return Together(api_key="804c67bbc002570c94c2e09da5ce7d1b575f852407fce7519c8ddd54be5aa5d6")


def ask_question(user_content, client, system_content):
    response = client.chat.completions.create(
        model="meta-llama/Llama-3-70b-chat-hf",
        messages=[
            {"role": "system", "content": system_content},
            {"role": "user", "content": user_content}
        ],
        # stream=True,
        temperature=0.7,
        top_k=50,
        top_p=0.6,
        max_tokens=200,
        n=1,
        stop=None
    )
    return response.choices[0].message.content