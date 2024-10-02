# app.py
from fastapi import FastAPI, Request
from transformers import AutoModelForCausalLM, AutoTokenizer

app = FastAPI()

# Load the pre-trained GPT-2 model and tokenizer from Hugging Face
model_name = "gpt2"
model = AutoModelForCausalLM.from_pretrained(model_name)
tokenizer = AutoTokenizer.from_pretrained(model_name)

@app.get("/")
async def root():
    return {"message": "Hugging Face API is running!"}

@app.post("/generate")
async def generate_text(request: Request):
    data = await request.json()
    prompt = data.get("prompt", "")

    # Tokenize the input prompt
    inputs = tokenizer(prompt, return_tensors="pt")

    # Generate text using the GPT-2 model
    outputs = model.generate(inputs.input_ids, max_length=100)

    # Decode the generated tokens into text
    generated_text = tokenizer.decode(outputs[0], skip_special_tokens=True)

    return {"generated_text": generated_text}
