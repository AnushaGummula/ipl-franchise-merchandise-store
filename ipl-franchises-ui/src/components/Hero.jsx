function Hero() {
  const shopNow = () => {
    document.getElementById('merchandise')?.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
    })
  }

  return (
    <section className="hero-banner">
      <img
        src="/images/hero/IPL_Hero_10_Teams_One_Passion.png"
        alt="10 teams one passion cricket merchandise banner"
        className="hero-banner-image"
      />

      <button
        type="button"
        className="hero-shop-overlay"
        onClick={shopNow}
        aria-label="Shop merchandise"
      />
    </section>
  )
}

export default Hero
