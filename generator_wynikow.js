import random
import csv

# Wrzucenie do słownika przedziałów odległości, na którą mogą latać zawodnicy na danych skoczniach 
przedzialy_odleglosci = {
    1: (105, 130),
    2: (110, 145),
    3: (180, 240),
}

# Przypisanie punktów konstrukcyjnych każdej ze skoczni, która potem ma wpływ na wyliczanie punktów za oddane skoki
punkty_konstrukcyjne = {
    1: 115,  
    2: 123,
    3: 200,  
}

# Generowanie losowych odległości i wrzucenie ich do listy
def oblicz_skok(skocznia):
    przedzial_odleglosci = przedzialy_odleglosci.get(skocznia, None)
    if przedzial_odleglosci is None:
        wielkosc_skoczni = skocznia * 5  # Zakładam, że przedziały będą zależne od skoczni
        przedzial_odleglosci = (wielkosc_skoczni * 0.5, wielkosc_skoczni * 0.5 + 10)

    # Generowanie odległości, uwzględniając tylko wartości całkowite i połówki
    odleglosc = round(random.uniform(*przedzial_odleglosci) * 2) / 2
    noty = [random.choice([17, 17.5, 18, 18.5, 19, 19.5, 20]) for _ in range(5)] 
    wiatr = round(random.uniform(-7, 7), 1)
    return [odleglosc] + noty + [wiatr]

def policz_punkty_za_skok(odleglosc, wiatr, skocznia, noty):
    punkty_skok = 60 if odleglosc == punkty_konstrukcyjne[skocznia] else (
        (odleglosc - punkty_konstrukcyjne[skocznia]) * 1.8 + 60 if odleglosc > punkty_konstrukcyjne[skocznia] else (
            60 - (punkty_konstrukcyjne[skocznia] - odleglosc) * 1.8 if odleglosc < punkty_konstrukcyjne[skocznia] else 60))
    suma_punktow = punkty_skok

    # Odrzucenie dwóch skrajnych not od 1 do 5
    noty = noty[:5]
    noty.remove(min(noty))
    noty.remove(max(noty))
    suma_not = sum(noty)
    suma_punktow += suma_not + wiatr
    suma_punktow = round(suma_punktow, 1)
    return suma_punktow

# Tworzenie ramki danych
columns = ['ID_Zawodow', 'ID_Skoczka', 'ID_Skoczni', 'Odleglosc1', 'Odleglosc2' ]
columns += [f'Nota{i}' for i in range(1, 11)]
columns += ['Wiatr1', 'Wiatr2', 'Punkty']

dane =  []

# Dane dotyczące skoczni, skoczków i zawodów
SKOCZNIA_SEKWENCJA = [1, 2, 3]
ZAWODY = [zawod for zawod in range(1, 6)]

skoczki_counter = 0

for zawody, skocznia in zip(ZAWODY, SKOCZNIA_SEKWENCJA): 
    #zawody
    wyniki = []
    indeky = {}
    # skok1
    skoczkowie = list(range(1, 6))
    for skoczek in skoczkowie:
        odleglosc, nota1, nota2, nota3, nota4, nota5, wiatr = oblicz_skok(skocznia)
        punkty = round(policz_punkty_za_skok(odleglosc, wiatr, skocznia, [nota1, nota2, nota3, nota4, nota5]))
        wiersz = [zawody, skoczek, skocznia, odleglosc, 0, nota1, nota2, nota3, nota4, nota5, 0, 0, 0, 0, 0, wiatr, 0, punkty]
        key = (zawody, skoczek, skocznia)
        indeky[key] = len(wyniki)
        wyniki.append(wiersz)
        skoczki_counter += 1
        if skoczki_counter == 5:
            break

    if skoczki_counter == 5:
        skoczki_counter = 0
        #skok2
        top_30 = sorted(wyniki, key=lambda x: x[-1], reverse=True)[:30]
        top_skoczkowie = [wiersz[1] for wiersz in top_30]
        for skoczek in skoczkowie:
            odleglosc, nota1, nota2, nota3, nota4, nota5, wiatr = oblicz_skok(skocznia)
            punkty = round(policz_punkty_za_skok(odleglosc, wiatr, skocznia, [nota1, nota2, nota3, nota4, nota5]))
            key = (zawody, skoczek, skocznia)
            index = indeky[key]
            wyniki[index][4] = odleglosc
            wyniki[index][10] = nota1
            wyniki[index][11] = nota2
            wyniki[index][12] = nota3
            wyniki[index][13] = nota4
            wyniki[index][14] = nota5
            wyniki[index][16] = wiatr
            wyniki[index][17] += round(punkty)
        # sort
        wyniki = sorted(wyniki, key=lambda x: x[-1], reverse=True)

        dane.extend(wyniki)

with open('dane_odleglosci.csv', 'w', newline='') as f:
    writer = csv.writer(f)
    writer.writerow(columns)
    writer.writerows(dane)
