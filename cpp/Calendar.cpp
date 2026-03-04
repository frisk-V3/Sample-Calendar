#include <QtWidgets>
#include <QMap>

// 1日分のセルを表すクラス（HTMLの<td>に相当）
class DayCell : public QFrame {
    Q_OBJECT
public:
    DayCell(int day, QString key, bool isToday, QWidget *parent = nullptr) : QFrame(parent), dayKey(key) {
        setFrameStyle(QFrame::Box | QFrame::Plain);
        auto *layout = new QVBoxLayout(this);

        // 日付ラベル（HTMLの.day-num）
        auto *label = new QLabel(QString::number(day));
        label->setAlignment(Qt::AlignCenter);
        if (isToday) label->setStyleSheet("background: yellow; border-radius: 15px;");
        layout->addWidget(label);

        // メモ入力欄（HTMLのtextarea）
        memoEdit = new QTextEdit();
        memoEdit->setPlaceholderText("予定...");
        layout->addWidget(memoEdit);

        // データの読み込み（localStorageの代わり）
        QSettings settings("MyCompany", "CalendarApp");
        memoEdit->setPlainText(settings.value(dayKey).toString());

        // 入力されたら即保存
        connect(memoEdit, &QTextEdit::textChanged, [this, settings = &settings]() mutable {
            QSettings s("MyCompany", "CalendarApp");
            s.setValue(dayKey, memoEdit->toPlainText());
        });
    }

    void setExpanded(bool expanded) {
        memoEdit->setVisible(expanded); // 簡易的に表示/非表示で再現
        this->setFixedHeight(expanded ? 200 : 80);
    }

private:
    QString dayKey;
    QTextEdit *memoEdit;
};

// メインウィンドウ（HTMLの.calendar-container）
class CalendarWin : public QWidget {
    Q_OBJECT
public:
    CalendarWin() {
        auto *mainLayout = new QVBoxLayout(this);
        auto *grid = new QGridLayout();

        // 曜日ヘッダー
        QStringList headers = {"日", "月", "火", "水", "木", "金", "土"};
        for(int i=0; i<7; ++i) grid->addWidget(new QLabel(headers[i]), 0, i);

        // 31日分並べる例（簡略化のため1日から開始）
        for(int d=1; d<=31; ++d) {
            QString key = QString("note-2024-3-%1").arg(d);
            auto *cell = new DayCell(d, key, (d == 4)); // 4日を今日とする
            grid->addWidget(cell, (d-1)/7 + 1, (d-1)%7);
            cells << cell;
        }
        mainLayout->addLayout(grid);
    }

private:
    QList<DayCell*> cells;
};

int main(int argc, char *argv[]) {
    QApplication a(argc, argv);
    CalendarWin w;
    w.show();
    return a.exec();
}
