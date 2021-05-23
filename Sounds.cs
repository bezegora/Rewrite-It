using System;
using WMPLib;

namespace Rewrite_It
{
    public class Sounds
    {
        private readonly WindowsMediaPlayer officeBackground = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer checkMode = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer chosenOption = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer click = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer doorClose = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer doorEnter = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer message = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer nextPhrase = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer paper = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer promotion = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer stamp = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer cancel = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer cursorHovered = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer cursorNotHovered = new WindowsMediaPlayer();

        private bool officeBackgroundLoop;

        public Sounds()
        {
            officeBackground.URL = @"Sounds\OfficeBackground.wav";
            officeBackground.settings.volume = 40;
            officeBackground.close();
            checkMode.URL = @"Sounds\CheckMode.wav";
            checkMode.close();
            click.URL = @"Sounds\Click.wav";
            click.close();
            doorClose.URL = @"Sounds\DoorClose.wav";
            doorClose.close();
            doorEnter.URL = @"Sounds\DoorEnter.wav";
            doorEnter.close();
            message.URL = @"Sounds\NewMessage.wav";
            message.close();
            nextPhrase.URL = @"Sounds\NextPhrase.wav";
            nextPhrase.close();
            paper.URL = @"Sounds\Paper.wav";
            paper.close();
            promotion.URL = @"Sounds\Promotion.wav";
            promotion.close();
            stamp.URL = @"Sounds\Stamp.wav";
            stamp.close();
            chosenOption.URL = @"Sounds\ChosenOption.wav";
            chosenOption.close();
            cancel.URL = @"Sounds\Cancel.wav";
            cancel.close();
            cursorHovered.URL = @"Sounds\CursorHovered.wav";
            cursorHovered.close();
            cursorNotHovered.URL = @"Sounds\CursorNotHovered.wav";
            cursorNotHovered.close();
        }

        private void Play(WindowsMediaPlayer sound) => sound.controls.play();

        private void Stop(WindowsMediaPlayer sound) => sound.close();

        public void Tick()
        {
            if (officeBackgroundLoop && officeBackground.playState != WMPPlayState.wmppsPlaying)
                Play(officeBackground);
        }

        public void PlayOfficeBackground() => officeBackgroundLoop = true;

        public void StopOfficeBackground()
        {
            officeBackgroundLoop = false;
            Stop(officeBackground);
        }

        public void PlayCheckMode() => Play(checkMode);

        public void PlayClick() => Play(click);

        public void PlayDoorClose() => Play(doorClose);

        public void PlayDoorEnter() => Play(doorEnter);

        public void PlayMessage() => Play(message);

        public void PlayNextPhrase() => Play(nextPhrase);

        public void PlayPaper() => Play(paper);

        public void PlayPromotion() => Play(promotion);

        public void PlayStamp() => Play(stamp);

        public void PlayChosenOption() => Play(chosenOption);

        public void PlayCancel() => Play(cancel);

        public void PlayCursorHovered() => Play(cursorHovered);

        public void PlayCursorNotHovered() => Play(cursorNotHovered);
    }
}
