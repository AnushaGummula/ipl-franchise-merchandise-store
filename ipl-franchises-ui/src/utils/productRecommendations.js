export const getRecommendations = (products, purchasedItems, limit = 4) => {
  if (!Array.isArray(products) || !Array.isArray(purchasedItems)) {
    return []
  }

  const purchasedIds = new Set(purchasedItems.map(item => Number(item.productId)))

  const purchasedFranchises = new Set(
    purchasedItems.map(item => item.franchiseCode).filter(Boolean)
  )

  const purchasedTypes = new Set(purchasedItems.map(item => item.productType).filter(Boolean))

  const candidates = products.filter(
    product => Number(product.stockQuantity) > 0 && !purchasedIds.has(Number(product.id))
  )

  const sameFranchiseDifferentType = candidates.filter(
    product =>
      purchasedFranchises.has(product.franchiseCode) && !purchasedTypes.has(product.productType)
  )

  const alreadySelected = new Set(sameFranchiseDifferentType.map(item => item.id))

  const sameFranchise = candidates.filter(
    product => purchasedFranchises.has(product.franchiseCode) && !alreadySelected.has(product.id)
  )

  sameFranchise.forEach(item => alreadySelected.add(item.id))

  const remaining = candidates.filter(product => !alreadySelected.has(product.id))

  return [...sameFranchiseDifferentType, ...sameFranchise, ...remaining].slice(0, limit)
}
