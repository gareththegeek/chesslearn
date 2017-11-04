from pystockfish import *

deep = Engine(depth=20)

def eval(position):
    deep.setposition(position)
    move = deep.bestmove()  # deep.eval()

    split = move["info"].split(" ")
    index = split.index("score")
    score = split[index + 2]

    print(int(score) / 100)

eval(['e2e4'])
eval(['e2e4, e7e5'])
eval(['e2e4, e7e5, g1f3'])