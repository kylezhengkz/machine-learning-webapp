import pickle
import time
import re
import numpy as np
from sklearn.model_selection import train_test_split
import sys
import tracemalloc
tracemalloc.start()

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

max_token_length = 250
max_embedding_length = max_token_length * 300
def load_embeddings(text):
    embeddings = np.empty(max_embedding_length, dtype=np.float32)
    text = tokenize(text)
    tokens = text.split()
    
    for i, token in enumerate(tokens):
        if (i >= max_token_length):
            break
        if (token in embedding_dictionary):
            embeddings[(i*300):((i+1)*300)] = embedding_dictionary[token]
        else:
            embeddings[(i*300):((i+1)*300)] = np.zeros((300,), dtype=np.float32)
    
    if (i < max_token_length - 1):
        room_left = ((max_token_length - 1) - i)*300
        embeddings[((i+1)*300):] = np.zeros((room_left,), dtype=np.float32)
    
    return embeddings

print(f"Bytes of memory used: {tracemalloc.get_traced_memory()[0]}")
    
X = np.empty((len(df) * 5, max_embedding_length), np.float32)
y = []
accumulator = []
chunk_size = 5
print(f"Rows of data {len(df)}")
print(f"Total data to process {len(df) * 5}")
for i, row in df.iterrows():
    for j in range(1, 6):
        embeddings = load_embeddings(row[f"{j}-Star Reviews"])
        assert embeddings.dtype == np.float32
        assert len(embeddings) == max_embedding_length
        accumulator.append(embeddings)
        y.append(j)
        
    if (len(accumulator) % chunk_size == 0):
        offset_i = (i + 1) * 5
        assert offset_i % chunk_size == 0, offset_i
        start_index = offset_i - chunk_size
        end_index = offset_i
        print(f"Populating X from index {start_index} to {end_index - 1}")
        X[start_index:end_index] = accumulator
        accumulator.clear()
    
if (len(accumulator) > 0):
    print(f"{len(accumulator)} data left to process")
    offset_i = (i + 1) * 5
    start_index = offset_i - len(accumulator)
    end_index = offset_i
    print(f"Populating X from index {start_index} to {end_index - 1}")
    X[start_index:end_index] = accumulator
    accumulator.clear()
        
assert X.dtype == np.float32
    
print(f"Bytes of memory used: {tracemalloc.get_traced_memory()[0]}")
    
assert (len(X) == len(y))
print(f"Bytes of memory used by df: {sys.getsizeof(df)}")
print(f"Bytes of memory used by embedding_dictionary: {sys.getsizeof(embedding_dictionary)}")

del df
del embedding_dictionary

import gc
gc.collect()

print(f"Bytes of memory used after deleting df and embedding_dictionary: {tracemalloc.get_traced_memory()[0]}")

print("Processing complete")
print(f"Bytes of memory used by X: {sys.getsizeof(X)}")
print(f"Bytes of memory used by y: {sys.getsizeof(y)}")

validation_split_index = int(0.9*len(X))
print(f"Validation split index: {validation_split_index}")

X_train = X[:validation_split_index]
X_val = X[validation_split_index:]
print(X_train)

y_train = y[:validation_split_index]
y_val = y[validation_split_index:]

assert len(X_train) == len(y_train)
assert len(X_val) == len(y_val)
assert len(X_train) + len(X_val) == len(X)

print(f"Bytes of memory used before deleting X and y: {tracemalloc.get_traced_memory()[0]}")

del X
del y
gc.collect()

print(f"Bytes of memory used after deleting X and y: {tracemalloc.get_traced_memory()[0]}")
print(f"Bytes of memory used by X_train: {sys.getsizeof(X_train)}")
print(f"Bytes of memory used by y_train: {sys.getsizeof(y_train)}")
print(f"Bytes of memory used by X_val: {sys.getsizeof(X_val)}")
print(f"Bytes of memory used by y_val: {sys.getsizeof(y_val)}")


