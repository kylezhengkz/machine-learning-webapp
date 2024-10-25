import pickle
import time
import re

t0 = time.time()
with open("../dataset/dataset.pkl", "rb") as f:
    df = pickle.load(f)
t1 = time.time()
print(f"{round(t1 - t0, 3)} seconds to load dataset")

def tokenize(text):
    text =  text.replace("'", "")
    text = re.sub(r"([^\w\s])", r" \1 ", text)
    text = text.replace("_", " _ ")
    text = re.sub(r"\s+", " ", text).strip()
    text = text.lower()
    return text

t0 = time.time()
with open("../glove/embedding_dictionary.pkl", "rb") as f:
    embedding_dictionary = pickle.load(f)
t1 = time.time()
print(f"{round(t1 - t0, 3)} seconds to load embedding dictionary")

