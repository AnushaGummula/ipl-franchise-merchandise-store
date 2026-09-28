import { ArrowRight } from 'lucide-react'

const categories = [
  {
    label: 'Jerseys',
    value: 'Jersey',
    price: 'From ₹1,499',
    image: '/images/products/CSK_Jersey.jpg',
    className: 'quick-category-yellow',
  },
  {
    label: 'Caps',
    value: 'Cap',
    price: 'From ₹699',
    image: '/images/products/MI_Cap.png',
    className: 'quick-category-blue',
  },
  {
    label: 'Flags',
    value: 'Flag',
    price: 'From ₹449',
    image: '/images/products/SRH_Flag.jpg',
    className: 'quick-category-red',
  },
]

function CategoryStrip({ selectedType, onTypeChange }) {
  const chooseCategory = value => {
    onTypeChange(selectedType === value ? '' : value)

    document.getElementById('merchandise')?.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
    })
  }

  return (
    <section className="quick-category-section" id="quick-categories">
      <div className="quick-category-grid">
        {categories.map(category => (
          <button
            key={category.value}
            type="button"
            className={`
              quick-category-card
              ${category.className}
              ${selectedType === category.value ? 'selected' : ''}
            `}
            onClick={() => chooseCategory(category.value)}
          >
            <div className="quick-category-copy">
              <strong>{category.label}</strong>

              <span>{category.price}</span>

              <small>
                Shop now
                <ArrowRight size={11} />
              </small>
            </div>

            <img src={category.image} alt={category.label} className="quick-category-image" />
          </button>
        ))}
      </div>
    </section>
  )
}

export default CategoryStrip
