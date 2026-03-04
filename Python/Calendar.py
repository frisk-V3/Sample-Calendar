import tkinter as tk
from tkinter import scrolledtext
import calendar
from datetime import datetime
import json
import os

class CalendarApp:
    def __init__(self, root):
        self.root = root
        self.root.title("メモ・カレンダー")
        self.root.geometry("800x600")
        
        self.curr_date = datetime.now()
        self.data_file = "calendar_data.json"
        self.memo_data = self.load_data()
        
        self.create_widgets()
        self.draw_calendar()

    def load_data(self):
        if os.path.exists(self.data_file):
            with open(self.data_file, 'r', encoding='utf-8') as f:
                return json.load(f)
        return {}

    def save_data(self, key, text):
        self.memo_data[key] = text
        with open(self.data_file, 'w', encoding='utf-8') as f:
            json.dump(self.memo_data, f, ensure_ascii=False)

    def create_widgets(self):
        self.header = tk.Frame(self.root)
        self.header.pack(pady=10)
        
        tk.Button(self.header, text="◀ 前月", command=lambda: self.change_month(-1)).pack(side=tk.LEFT)
        self.month_label = tk.Label(self.header, text="", font=("Arial", 16, "bold"))
        self.month_label.pack(side=tk.LEFT, padx=20)
        tk.Button(self.header, text="次月 ▶", command=lambda: self.change_month(1)).pack(side=tk.LEFT)

        self.cal_frame = tk.Frame(self.root)
        self.cal_frame.pack(expand=True, fill="both", padx=10, pady=10)

    def draw_calendar(self):
        for widget in self.cal_frame.winfo_children():
            widget.destroy()

        y, m = self.curr_date.year, self.curr_date.month
        self.month_label.config(text=f"{y}年 {m}月")

        # 曜日ヘッダー
        days = ["日", "月", "火", "水", "木", "金", "土"]
        for i, d in enumerate(days):
            color = "red" if i == 0 else "blue" if i == 6 else "black"
            tk.Label(self.cal_frame, text=d, fg=color, font=("Arial", 10, "bold")).grid(row=0, column=i, sticky="nsew")

        cal = calendar.Calendar(firstweekday=6)
        weeks = cal.monthdayscalendar(y, m)

        for r, week in enumerate(weeks):
            for c, day in enumerate(week):
                if day == 0: continue
                
                cell = tk.Frame(self.cal_frame, bd=1, relief="sunken")
                cell.grid(row=r+1, column=c, sticky="nsew")
                
                # 数字の色分け
                num_color = "red" if c == 0 else "blue" if c == 6 else "black"
                tk.Label(cell, text=day, fg=num_color, font=("Arial", 10)).pack(anchor="nw")

                # メモ入力欄
                key = f"{y}-{m}-{day}"
                txt = scrolledtext.ScrolledText(cell, height=3, width=10, font=("Arial", 8))
                txt.insert(tk.END, self.memo_data.get(key, ""))
                txt.pack(expand=True, fill="both")
                
                # 入力されたら自動保存
                txt.bind("<KeyRelease>", lambda e, k=key, t=txt: self.save_data(k, t.get("1.0", tk.END).strip()))

        for i in range(7): self.cal_frame.grid_columnconfigure(i, weight=1)
        for i in range(len(weeks)+1): self.cal_frame.grid_rowconfigure(i, weight=1)

    def change_month(self, diff):
        new_month = self.curr_date.month + diff
        new_year = self.curr_date.year
        if new_month > 12:
            new_month = 1
            new_year += 1
        elif new_month < 1:
            new_month = 12
            new_year -= 1
        self.curr_date = datetime(new_year, new_month, 1)
        self.draw_calendar()

if __name__ == "__main__":
    root = tk.Tk()
    app = CalendarApp(root)
    root.mainloop()
