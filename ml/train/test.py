import numpy as np
import pickle
import tensorflow as tf
import re

# Load the model
model = tf.keras.models.load_model("../models/test_model.h5")

# Load the GloVe embedding dictionary
with open("../glove/embedding_dictionary.pkl", "rb") as f:
    embedding_dictionary = pickle.load(f)

# Parameters
max_token_length = 250
max_embedding_length = max_token_length * 300

def tokenize(text):
    """Tokenizes and preprocesses input text."""
    text = text.replace("'", "")
    text = re.sub(r"([^\w\s])", r" \1 ", text)
    text = text.replace("_", " _ ")
    text = re.sub(r"\s+", " ", text).strip()
    text = text.lower()
    return text

def load_embeddings(text):
    """Converts text to embeddings using GloVe."""
    embeddings = np.zeros(max_embedding_length, dtype=np.float32)
    tokens = tokenize(text).split()
    
    for i, token in enumerate(tokens):
        if i >= max_token_length:
            break
        if token in embedding_dictionary:
            embeddings[i*300:(i+1)*300] = embedding_dictionary[token]
    
    return embeddings

def main():
    """Runs the sentiment analysis model on user input."""
    while True:
        text = input("Enter text for sentiment analysis (or type 'exit' to quit): ")
        if text.lower() == "exit":
            break

        # Process text input to embeddings
        input_embeddings = load_embeddings(text)
        input_embeddings = np.expand_dims(input_embeddings, axis=0)  # Reshape for model input
        
        # Predict with the model
        prediction = model.predict(input_embeddings)
        
        # Print the result
        print(f"Predicted Sentiment: {prediction[0][0]}")

if __name__ == "__main__":
    main()
