# Readme

Hello dear programmer

If you have looked at the codes already, you might have said a lot of wtfs.

I have used too many bad things during the writing of this project, including a lot of global states, 
mutating things that should have been readonly and constant, some circular dependancies, etc. Also my code
lacks DI and some other useful patterns.

I accept the blames on me, but this was due to the deadline and my lack of time (also it is too easy to write
bad codes in C#).

Forgive the code.

Best regards, the project creator.

# Docs

### 3rd-Party Input Format
Applicaton inputs a file to read from a 3rd-party app with this format:

```
A,B,C,D,E
```
where:
```
A: PackNumber
B: Date (in PersianCalendar format)
C: Time
D: Weight
E: Item-code Id in `Goods` database
```
Example:
```
12345689,13980121,1210,1260,7080130002
```

**Configuration Address**

`Config.PackDetailsFileAddress`

**Parsing Function**

File: `App.xaml.cs`

Function: `RecivePackDetailsDataAsync : unit -> Task`