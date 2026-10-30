import React, { useRef, useEffect } from 'react';
import { Box } from '@mui/material';
import ReactPlayer from 'react-player';

interface VideoPlayerProps {
  url: string;
  onProgress?: (progress: number) => void;
  onEnded?: () => void;
  initialTime?: number;
}

const VideoPlayer: React.FC<VideoPlayerProps> = ({
  url,
  onProgress,
  onEnded,
  initialTime = 0,
}) => {
  const playerRef = useRef<ReactPlayer>(null);

  useEffect(() => {
    if (playerRef.current && initialTime > 0) {
      playerRef.current.seekTo(initialTime, 'seconds');
    }
  }, [initialTime]);

  const handleProgress = (state: { playedSeconds: number }) => {
    if (onProgress) {
      onProgress(state.playedSeconds);
    }
  };

  return (
    <Box sx={{ position: 'relative', paddingTop: '56.25%', backgroundColor: '#000' }}>
      <ReactPlayer
        ref={playerRef}
        url={url}
        width="100%"
        height="100%"
        controls
        onProgress={handleProgress}
        onEnded={onEnded}
        config={{
          youtube: {
            playerVars: { showinfo: 1 }
          }
        }}
        style={{
          position: 'absolute',
          top: 0,
          left: 0,
        }}
      />
    </Box>
  );
};

export default VideoPlayer;