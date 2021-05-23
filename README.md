# Rewrite-It
## Команда Booklet

**Формат системы:**
desktop приложение

**Цель проекта:**
Закрепление шаблонов ошибок, которые возникают при составлении текстов

**Описание:**
Игрок примет на себя роль главного редактора издательского центра “Истина”, 
обязанность которого заключается в рассмотрении статей различного характера (научные, рекламные, новостные) и отклонении или одобрении их к публикации.

**Целевая аудитори:**
Люди молодого и зрелого возраста стран СНГ

**Основное преимущество:**
Предусмотрен уклон на образовательный контент: пользователь не только использует приложение с целью развлечения,
но так или иначе обучается полезным навыкам копирайтинга и орфографии

**Стек технологий:**
Windows Forms (является частью .NET Framework)

**Работа пользователя с системой:**
//TODO

**Основные требования к ПО для использования:**

Иметь:
* Visual Studio 3.5 — 4.8 или выше **ИЛИ**
* JetBrains Rider 2021 или выше

**Структура приложения:**

Классы:
* Program.cs — главная точка входа для приложения
* Form1.cs — окно, составляющее интерфейс пользовательского приложения
* Controller.cs — главный класс, содержащий всю игровую логику и регулирующий переходы между интерфейсами
* GraphicObject.cs — класс, представляющий графический объект, имеющий способность передвигаться
* MainOffice.cs — класс, представляющий интерфейс офиса и все логические взаимодействия между элементами этого интерфейса
* CheckMode.cs — класс, представляющий интерфейс режима проверки и все логические взаимодействия между элементами этого интерфейса
* Email.cs — класс, представляющий интерфейс электронной почты и все логические взаимодействия между элементами этого интерфейса
* DayEnd.cs — класс, представляющий интерфейс подведения итогов уровня и являющийся переходным звеном между уровнями
* GameEvents.cs — статический класс, содержащий все возможные игровые события
* GameStats.cs — класс, содержащий информацию о состоянии игры и показателях, которые изменяются на протяжении всего игрового процесса
* LevelParameters.cs — класс, содержащий информацию о текущем уровне и показателях, характерных только для данного уровня
* Character.cs — класс, представляющий отображение персонажа и перемещение его по форме
* Phrase.cs — класс, содержащий информацию об одной фразе персонажа
* Mistake.cs — класс, содержащий информацию о типе ошибки, которая может находиться в текстовой области
* Book.cs — класс, хранящий информацию о содержимом книги в режиме проверки (перечни, руководство) и вкладках
* ExitButton.cs — класс, представляющий кнопку выхода из режима проверки
* StringStyle.cs — статический класс, содержащий информацию о стиле текста (шрифт, цвет), используемого в игре
* AuxiliaryMethods.cs — статический класс, содержащий вспомогательные методы, которые используются многими другими классами
* Sounds.cs — класс, содержащий все звуки, которые представлены в виде проигрывателей Windows Media Player

Перечисления:
* CharacterImage.cs — перечисление имён изображений персонажей
* Interface.cs — перечисление всех имеющихся в игре интерфейсов
* MistakeType.cs — перечисление типов ошибок
* Tab.cs — перечисление вкладок книги в режиме проверки

Шрифты:
* PixelGeorgia.ttf — стандартный стиль шрифта, используемого в игре

Папки:
* Articles — содержит текстовые файлы формата .txt — используемые в игре статьи
* Sounds — содержит звуковые файлы формата .wav — используемые в игре звуки
* Resources — содержит графические файлы формата .png — используемые в игре изображения
