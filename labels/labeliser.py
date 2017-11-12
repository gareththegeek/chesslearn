from pystockfish import *
import csv
import time

deep = Engine(depth=10)

inputFile = "KingBase2017-A00-A39 a-FENS.csv"
outputFile = "KingBase2017-A00-A39 a-LABELLED.csv"

def eval(position):
    deep.setfenposition(position)
    move = deep.bestmove()

    if move == None:
        return None

    split = move["info"].split(" ")
    index = split.index("score")
    score = split[index + 2]

    return int(score) / 100

#fails somewhere around 2520

start = time.clock()
with open(inputFile, "rt") as fin, open(outputFile, "wt", newline="") as fout:
    reader = csv.reader(fin, delimiter=",")
    writer = csv.writer(fout)
    next(reader, None) # Skip headings

    # for j in range(0, 506882):
    #     next(reader, None)

    i = 0
    for row in reader:
        try:
            fen = row[0]
            #print(fen)
            isWhite = fen.split(" ")[1] == "b"
            score = eval(fen)

            if score == None:
                continue

            if isWhite:
                score = -score

            row = [fen]+[str(score)]
            writer.writerow(row)
            #print("[" + str(i) + "]: " + ", ".join(row))
            i = i + 1
            if(i%100==0):
                print("Written " + str(i) + " rows in " + str(time.clock() - start))
        except BaseException as e:
            print("Error " + str(e))


print("Complete. " + str(i) + " rows total " + str(time.clock() - start) + " seconds")
