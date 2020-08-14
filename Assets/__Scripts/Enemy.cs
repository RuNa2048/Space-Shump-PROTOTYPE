using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[Header("Set in Inspector: Enemy")]
	public float speed = 10f;
	public float fireRate = 0.3f;
	public float health = 10;
	public int score = 100;
	public float showDamageDuration = 0.1f;

	public float powerUpDropChance = 1f;

	[Header("Set Dynamically")]
	public Color[] originalColor;
	public Material[] materials;
	public bool showingDamage = false;
	public float damageDoneTime;
	public bool notifiedOfdestruction = false;

	protected BoundsCheck bndCheck;

	public Vector3 pos
	{
		get
		{
			return (this.transform.position);
		}
		set
		{
			this.transform.position = value;
		}
	}

	private void Awake()
	{
		bndCheck = GetComponent<BoundsCheck>();
		materials = Utils.GetAllMaterials(this.gameObject);
		originalColor = new Color[materials.Length];
		for (int i = 0; i < materials.Length; i++)
		{
			originalColor[i] = materials[i].color;
		}
	}

	private void Update()
	{
		Move();

		if (showingDamage && Time.time > damageDoneTime)
		{
			UnShowDamage();
		}

		if (bndCheck != null && bndCheck.offDown)
		{
			Destroy(this.gameObject);
		}
	}

	public virtual void Move()
	{
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}

	private void OnCollisionEnter(Collision collision)
	{
		GameObject otherGO = collision.gameObject;
		switch (otherGO.tag)
		{
			case "ProjectileHero":
				Projectile p = otherGO.GetComponent<Projectile>();
				if (!bndCheck.isOnScreen)
				{
					Destroy(otherGO);
					break;
				}
				health -= Main.GetWeaponDefinition(p.type).damageOnHit;
				ShowDamage();
				if (health <= 0)
				{
					if (!notifiedOfdestruction)
					{
						Main.S.ShipDestroyed(this);
					}
					notifiedOfdestruction = true;

					Destroy(this.gameObject);
				}
				Destroy(otherGO);
			break;

			default:
				print("Enemy hit by non-ProjectileHero:" + otherGO.name);
				break;
		}
	}

	private void ShowDamage()
	{
		foreach (Material m in materials)
		{
			m.color = Color.red;
		}
		showingDamage = true;
		damageDoneTime = Time.time + showDamageDuration;
	}

	private void UnShowDamage()
	{
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].color = originalColor[i];
		}
		showingDamage = false;
	}
}
