
function Grid({ squaresPerSide = 16 }) {
  // Generate an array of size squaresPerSide * squaresPerSide
  const totalSquares = squaresPerSide * squaresPerSide;
  
  const containerStyle = {
    display: "flex",
    flexWrap: "wrap",
    width: "480px",
    height: "480px",
    backgroundColor: "black",
    marginTop: "10px",
    marginBottom: "auto",
  } as const;

  const squareSize = `${480 / squaresPerSide}px`;

  return (
    <div style={containerStyle}>
      {Array.from({ length: totalSquares }).map((_, index) => (
        <div
          key={index}
          style={{
            width: squareSize,
            height: squareSize,
            backgroundColor: "gray",
          }}
        />
      ))}
    </div>
  );
}

export default Grid;