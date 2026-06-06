#!/usr/bin/env python3
"""Generate a gentle looping music bed + a completion fanfare for Count the Fruits."""
import os, wave
import numpy as np

AUD = os.path.join(os.path.dirname(__file__), "..", "Assets", "Audio")
os.makedirs(AUD, exist_ok=True)
SR = 44100

def note(freq):
    return freq

# note frequencies
A4=440.0
def n(name):
    names={'C':-9,'C#':-8,'D':-7,'D#':-6,'E':-5,'F':-4,'F#':-3,'G':-2,'G#':-1,
           'A':0,'A#':1,'B':2}
    # name like 'C4'
    p=name[:-1]; octv=int(name[-1])
    semis=names[p]+(octv-4)*12
    return A4*(2**(semis/12))

def tone(freq, dur, kind='sine', decay=0.0, attack=0.005):
    t=np.linspace(0,dur,int(SR*dur),endpoint=False)
    if kind=='sine': w=np.sin(2*np.pi*freq*t)
    elif kind=='tri': w=2*np.abs(2*(t*freq-np.floor(t*freq+0.5)))-1
    else: w=np.sin(2*np.pi*freq*t)
    env=np.ones_like(t)
    a=int(SR*attack)
    if a>0: env[:a]=np.linspace(0,1,a)
    if decay>0: env*=np.exp(-t*decay)
    else:
        r=int(SR*0.02); env[-r:]*=np.linspace(1,0,r)  # tiny release
    return w*env

def write_wav(path, sig):
    sig=np.clip(sig,-1,1)
    pcm=(sig*32767).astype(np.int16)
    with wave.open(path,'w') as w:
        w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR); w.writeframes(pcm.tobytes())
    print("wrote", os.path.basename(path), f"{len(sig)/SR:.1f}s")

def pad_chord(freqs, dur):
    """Soft swelling pad: attack in, release out -> seam-safe per bar."""
    seg=np.zeros(int(SR*dur))
    t=np.linspace(0,dur,len(seg),endpoint=False)
    env=np.sin(np.pi*t/dur)**1.2  # swell up then down to 0 at both ends
    for f in freqs:
        seg+=np.sin(2*np.pi*f*t)
    seg/=len(freqs)
    return seg*env

def gen_music_bed(path):
    bar=2.0
    # I  V  vi IV  in C major
    chords=[['C3','E3','G3'], ['G2','B2','D3'], ['A2','C3','E3'], ['F2','A2','C3']]
    # pentatonic arpeggio tones (C major pentatonic), per bar
    arps=[['C4','E4','G4','E4'], ['D4','G4','B4','G4'], ['E4','A4','C5','A4'], ['C4','F4','A4','F4']]
    out=np.zeros(int(SR*bar*len(chords)))
    for bi,(ch,ar) in enumerate(zip(chords,arps)):
        off=int(SR*bar*bi)
        # pad
        p=pad_chord([n(x) for x in ch], bar)*0.45
        out[off:off+len(p)]+=p
        # bass (root, soft)
        b=tone(n(ch[0])/2, bar, 'sine', decay=0.0)*0.22
        out[off:off+len(b)]+=b
        # arpeggio: 8 eighth notes (cycle the 4 tones twice)
        step=bar/8
        seq=ar+ar
        for si,name in enumerate(seq):
            a=tone(n(name), step*0.95, 'tri', decay=6.0, attack=0.004)*0.16
            so=off+int(SR*step*si)
            out[so:so+len(a)]+=a
    # normalize gently (background level)
    out=out/np.max(np.abs(out))*0.55
    # ensure seam continuity (both ends ~0 already from pad swell)
    write_wav(path, out)

def gen_fanfare(path):
    seq=[('C4',0.12),('E4',0.12),('G4',0.12),('C5',0.16),('E5',0.16),('G5',0.30)]
    out=np.array([])
    for name,dur in seq:
        s=tone(n(name),dur,'tri',decay=4.0,attack=0.004)*0.8
        # add a soft fifth for richness
        s=s+tone(n(name)*1.5,dur,'sine',decay=5.0)*0.2
        out=np.concatenate([out,s])
    out=out/np.max(np.abs(out))*0.85
    write_wav(path, out)

if __name__=='__main__':
    gen_music_bed(os.path.join(AUD,'music_bed.wav'))
    gen_fanfare(os.path.join(AUD,'fanfare.wav'))
    print("done")
