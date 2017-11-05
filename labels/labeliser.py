from pystockfish import *
import csv
import time

deep = Engine(depth=20)

inputFile = "outfile.csv"
outputFile = "labelled.csv"

def eval(position):
    deep.setfenposition(position)
    move = deep.bestmove()

    split = move["info"].split(" ")
    index = split.index("score")
    score = split[index + 2]

    return int(score) / 100

start = time.clock()
with open(inputFile, "rt") as fin, open(outputFile, "wt", newline="") as fout:
    reader = csv.reader(fin, delimiter=",")
    writer = csv.writer(fout)
    next(reader, None) # Skip headings
    i = 0
    for row in reader:
        fen = row[1]
        score = eval(fen)
        if (row[2] == "0"):
            score = -score
        row = row[2:]+[str(score)]
        writer.writerow(row)
        #print("[" + str(i) + "]: " + ", ".join(row))
        i = i + 1
        if(i%10==0):
            print("Written " + str(i) + " rows in " + str(time.clock() - start))

print("Complete. " + str(i) + " rows total " + str(time.clock() - start) + " seconds")
