let coinInterval = null;
let container = null;

export function startCoins() {
    if (container) return;

    container = document.createElement('div');
    container.style.position = 'fixed';
    container.style.top = '0';
    container.style.left = '0';
    container.style.width = '100%';
    container.style.height = '100%';
    container.style.pointerEvents = 'none';
    container.style.overflow = 'visible';
    container.style.zIndex = '9999';
    document.body.appendChild(container);

    const totalFrames = 9;
    const basePath = '/assets/goldCoin';
    const extension = '.png';

    function createCoin() {
        const coin = document.createElement('img');
        const size = Math.random() * 15 + 20;
        coin.style.position = 'absolute';
        coin.style.top = '-20px';
        coin.style.left = Math.random() * window.innerWidth + 'px';
        coin.style.width = size + 'px';
        coin.style.height = size + 'px';
        coin.style.opacity = Math.random() * 0.7 + 0.3;
        coin.style.pointerEvents = 'none';
        coin.style.userSelect = 'none';

        container.appendChild(coin);

        const duration = Math.random() * 3000 + 2000;
        let start = null;
        const frameDuration = 100;

        function animate(timestamp) {
            if (!start) start = timestamp;
            const elapsed = timestamp - start;
            const progress = elapsed / duration;

            coin.style.top = progress * window.innerHeight + 'px';
            coin.style.opacity = 1 - progress;

            const frameIndex = Math.floor(elapsed / frameDuration) % totalFrames + 1;
            coin.src = `${basePath}${frameIndex}${extension}`;

            if (progress < 1) {
                requestAnimationFrame(animate);
            } else {
                if (coin.parentNode) {
                    coin.parentNode.removeChild(coin);
                }
            }
        }
        requestAnimationFrame(animate);
    }

    coinInterval = setInterval(createCoin, 50);
}

export function stopCoins() {
    if (coinInterval) {
        clearInterval(coinInterval);
        coinInterval = null;
    }
    if (container && container.parentNode) {
        container.parentNode.removeChild(container);
        container = null;
    }
}

window.startCoins = startCoins;
window.stopCoins = stopCoins;
